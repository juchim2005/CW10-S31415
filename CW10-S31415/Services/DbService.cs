using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CW10_S31415.Data;
using CW10_S31415.DTOs;
using CW10_S31415.Exceptions;
using CW10_S31415.Models;
using Microsoft.EntityFrameworkCore;

namespace CW10_S31415.Services;

public interface IDbService
{
    public Task<TripPageGetDto> GetTripsPagedAsync(int page, int pageSize);
    public Task<ICollection<TripGetDto>> GetTripsAsync();
    public Task DeleteClientAsync(int idClient);
    public Task AddClientToTripAsync(int id, ClientTripCreateDto body);
}

public class DbService(CW10Context data) : IDbService
{
    public async Task<TripPageGetDto> GetTripsPagedAsync(int page, int pageSize)
    {
        var trips = await GetTripsAsync();
        var size = trips.Count;
        var numberOfPages = size / pageSize;
        if (size % pageSize != 0) numberOfPages++;
        
        if (page > numberOfPages) page = numberOfPages;
        
        var returnedTrips = trips.Skip((page-1) * pageSize).Take(pageSize).ToList();
        return new TripPageGetDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = numberOfPages,
            Trips = returnedTrips
        };
    }

    public async Task<ICollection<TripGetDto>> GetTripsAsync()
    {
        return await data.Trips.Select(t => new TripGetDto
        {
            Name = t.Name,
            Description = t.Description,
            DateFrom = t.DateFrom,
            DateTo = t.DateTo,
            MaxPeople = t.MaxPeople,
            Countries = t.IdCountries.Select(c => new CountryGetDto
            {
                Name = c.Name
            }).ToList(),
            Clients = t.ClientTrips
                .Where(ct=>ct.IdTrip == t.IdTrip)
                .Select(ct => new  ClientTripGetDto
                {
                    FirstName = ct.IdClientNavigation.FirstName,
                    LastName = ct.IdClientNavigation.LastName
                }).ToList()
        }).OrderByDescending(t=>t.DateFrom).ToListAsync();
    }

    public async Task DeleteClientAsync(int idClient)
    {
        var client = await data.Clients
            .Include(client => client.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);
        if (client == null)
        {
            throw new NotFoundException($"Client with id {idClient} not found");
        }

        if (client.ClientTrips.Count != 0)
        {
            throw new BadRequestException($"Client with id {idClient} has atleast one trip");
        }
        
        data.Clients.Remove(client);
        await data.SaveChangesAsync();
    }

    public async Task AddClientToTripAsync(int id,ClientTripCreateDto body)
    {
        var trip = await data.Trips.FirstOrDefaultAsync(t => t.IdTrip == id);
        if (trip == null)
        {
            throw new NotFoundException($"Trip with id {id} not found");
        }

        if (trip.DateFrom < DateTime.Now)
        {
            throw new BadRequestException($"Trip with id {id} has expired");
        }
        
        var client = await data.Clients.FirstOrDefaultAsync(c => c.Pesel == body.Pesel);
        if (client != null)
        {
            throw new BadRequestException($"Client with PESEL {body.Pesel} already exists");
        }
        
        var clientTrip = await data.ClientTrips.FirstOrDefaultAsync(ct => ct.IdClientNavigation.Pesel == body.Pesel && ct.IdTrip == id);
        if (clientTrip != null)
        {
            throw new BadRequestException($"Client with PESEL {body.Pesel} already is signed on trip with id {id}");
        }

        var clientCreate = new Client
        {
            FirstName = body.FirstName,
            LastName = body.LastName,
            Email = body.Email,
            Telephone = body.Telephone,
            Pesel = body.Pesel
        };
        data.Clients.Add(clientCreate);
        await data.SaveChangesAsync();

        var clientTripCreate = new ClientTrip
        {
            IdClient = clientCreate.IdClient,
            IdTrip = id,
            RegisteredAt = DateTime.Now,
            PaymentDate = body.PaymentDate
        };
        data.ClientTrips.Add(clientTripCreate);
        await data.SaveChangesAsync();
    }
}
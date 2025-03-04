using AppUI.Services;
using Shared.Infrastructure.Database.Entities;

namespace AppUI.ViewModels;

public class PersonViewModel(CancellationTokenSource cts, MinimalDataService dataService)
{
    private readonly CancellationToken cancellationToken = cts.Token;

    public List<PersonEntity> People = [];

    public HashSet<AddressEntity> SelectedAddresses { get; set; } = [];
    public HashSet<TelephoneConnectionEntity> SelectedConnections { get; set; } = [];
    public HashSet<PersonEntity> SelectedPeople { get; set; } = [];

    public async Task FetchPeople()
    {
        this.People.Clear();

        // clear selected
        this.SelectedAddresses.Clear();
        this.SelectedConnections.Clear();
        this.SelectedPeople.Clear();

        this.People.AddRange(await dataService.GetAllPeople(this.cancellationToken));
    }

    public async Task CreatePerson(PersonEntity person)
    {
        var creationResult = await dataService.CreatePerson(person, this.cancellationToken);

        this.People.Add(creationResult);
    }

    public async Task<List<string>> DeleteAddresses(PersonEntity person)
    {
        if (this.SelectedAddresses.Count <= 0)
        {
            return [];
        }

        var list = new List<string>();

        foreach (var address in this.SelectedAddresses)
        {
            try
            {
                await dataService.DeleteAddress((uint)address.Id, this.cancellationToken);

                person.Addresses.Remove(address);
                this.SelectedAddresses.Add(address);
            }
            catch (Exception)
            {
                list.Add(address.ToString() ?? address.Id.ToString());

                continue;
            }
        }

        return list;
    }

    public async Task<List<string>> DeleteConnections(PersonEntity person)
    {
        if (this.SelectedConnections.Count <= 0)
        {
            return [];
        }

        var list = new List<string>();

        foreach (var connection in this.SelectedConnections)
        {
            try
            {
                await dataService.DeleteConnection((uint)connection.Id, this.cancellationToken);

                person.TelephoneConnections.Remove(connection);
                this.SelectedConnections.Add(connection);
            }
            catch (Exception)
            {
                list.Add(connection.PhoneNumber);

                continue;
            }
        }

        return list;
    }

    public async Task<List<string>> DeletePeople()
    {
        if (this.SelectedPeople.Count <= 0)
        {
            return [];
        }

        var personErrors = new List<string>();

        foreach (var person in this.SelectedPeople)
        {
            if (person.TelephoneConnections.Count > 0 || person.Addresses.Count > 0)
            {
                personErrors.Add((person.ToString()
                    ?? person.Id.ToString()) + "(Existierende Telefonverbindungen oder Adressen vorhanden)");

                continue;
            }

            try
            {
                await dataService.DeletePerson((uint)person.Id, this.cancellationToken);

                this.People.Remove(person);
                this.SelectedPeople.Remove(person);
            }
            catch (Exception)
            {
                personErrors.Add(person.ToString() ?? person.Id.ToString());

                continue;
            }
        }

        return personErrors;
    }

    public Task UpdatePerson(PersonEntity person)
    {
        return dataService.UpdatePerson(person, this.cancellationToken);
    }

    public Task UpdateAddress(AddressEntity address)
    {
        return dataService.UpdateAddress(address, this.cancellationToken);
    }

    public Task UpdateConnection(TelephoneConnectionEntity connection)
    {
        return dataService.UpdateConnection(connection, this.cancellationToken);
    }
}

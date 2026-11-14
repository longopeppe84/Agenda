using Agenda2.Shared.DTOs;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Agenda2.Client.Pages
{
    public partial class Contacts
    {
        [Inject]
        public HttpClient Http { get; set; } = default!;

        private IEnumerable<ContactDto>? contacts;
        private bool showDeleteModal = false;
        private bool isDeleting = false;
        private ContactDto? contactToDelete;

        protected override async Task OnInitializedAsync() => await LoadContacts();

        private async Task LoadContacts() => contacts = await Http.GetFromJsonAsync<IEnumerable<ContactDto>>("api/contact");

        private void OpenDeleteModal(ContactDto contact)
        {
            contactToDelete = contact;
            showDeleteModal = true;
        }

        private void CloseDeleteModal()
        {
            showDeleteModal = false;
            contactToDelete = null;
        }

        private async Task ConfirmDelete()
        {
            if (contactToDelete != null)
            {
                try
                {
                    isDeleting = true;
                    await Http.DeleteAsync($"api/contact/{contactToDelete.Id}");
                    CloseDeleteModal();
                    await LoadContacts();
                }
                finally
                {
                    isDeleting = false;
                }
            }
        }
    }
}
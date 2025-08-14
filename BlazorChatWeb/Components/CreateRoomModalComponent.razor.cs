using BlazorChatShared.Models.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorChatWeb.Components;

public partial class CreateRoomModalComponent : ComponentBase
{
    [Parameter] public string Title { get; set; } = "New title";
    [Parameter] public string Description { get; set; } = "New description";
    [Parameter] public EventCallback<Room> OnSubmit { get; set; }

    private Room room = new Room();

    private ElementReference ModalElement;
    private bool _isRendered = false;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _isRendered = true;
        }
    }

    public async Task ShowAsync()
    {
        if (_isRendered)
        {
            await JS.InvokeVoidAsync("modalHelper.showModal", ModalElement);
        }
        else
        {
            Console.Error.WriteLine("Modal not yet rendered.");
        }
    }

    public async Task CloseModal()
    {
        if (_isRendered)
        {
            await JS.InvokeVoidAsync("modalHelper.hideModal", ModalElement);
        }
    }

    private async Task HandleSubmit()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(room);
        }
        await CloseModal();

        // Reset form for next time
        room = new Room();
        StateHasChanged();
    }
}

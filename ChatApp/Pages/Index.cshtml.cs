using ChatApp.Interfaces;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class IndexModel : PageModel
{
    private readonly IChatService _chatService;
    private readonly IBotService _botService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IChatService chatService, IBotService botService,
        UserManager<IdentityUser> userManager, ILogger<IndexModel> logger)
    {
        _chatService = chatService;
        _botService = botService;
        _userManager = userManager;
        _logger = logger;
    }

    public IEnumerable<ChatMessage> Messages { get; set; }
    public IEnumerable<ChatRoom> ChatRooms { get; set; }
    public int CurrentChatRoomId { get; set; }

    [BindProperty]
    public string NewMessage { get; set; }

    public async Task OnGetAsync(int? currentChatRoomId)
    {
        ChatRooms = _chatService.GetChatRooms();
        CurrentChatRoomId = currentChatRoomId ?? ChatRooms.FirstOrDefault()?.Id ?? 0;
        Messages = _chatService.GetRecentMessages(CurrentChatRoomId);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var message = new ChatMessage
            {
                UserId = user.Id,
                Username = user.UserName,
                Message = NewMessage,
                Timestamp = DateTime.UtcNow,
                ChatRoomId = CurrentChatRoomId
            };

            if (message.Message.StartsWith("/stock="))
            {
                await _botService.ProcessCommandAsync(message.Message);
            }
            else
            {
                _chatService.AddMessage(message);
            }

            // Reload messages
            Messages = _chatService.GetRecentMessages(CurrentChatRoomId);
        }

        return Page();
    }
}

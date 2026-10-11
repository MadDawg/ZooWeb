using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using ZooWeb.Pages.Employees;
using ZooWeb.Data;
using ZooWeb.Data.Entities;

namespace ZooWeb.Pages.Home
{
  public class PrivacyModel : PageModel
  {
    private readonly ILogger<PrivacyModel> _logger;
    private readonly ZooUserRepository _zooUserRepo;
    private readonly NotificationRepository _notificationRepo;

    public PrivacyModel(ILogger<PrivacyModel> logger, ZooUserRepository zooUserRepo, NotificationRepository notificationRepo)
    {
      _logger = logger;
      _zooUserRepo = zooUserRepo;
      _notificationRepo = notificationRepo;
    }

    public List<NotificationInfo> notifications = new List<NotificationInfo>();
    public async Task<IActionResult> OnGet()
    {
      string username = User?.FindFirstValue(ClaimTypes.Name);

      if (!string.IsNullOrEmpty(username))
      {
        int? userid = await _zooUserRepo.GetIdByUsernameAsync(username);
        if (userid is not null){ notifications = await _notificationRepo.GetByUserIdAsync((int)userid); }
      }
      return Page();
    }
    public async Task<IActionResult> OnPostDelete(int id){
      await _notificationRepo.DeleteByIdAsync(id);
      return RedirectToPage();
    }
  }
}

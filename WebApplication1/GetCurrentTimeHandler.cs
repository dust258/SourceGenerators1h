using Microsoft.AspNetCore.Mvc;

namespace WebApplication1;

public class GetCurrentTimeHandler
{
    [HttpGet("time")]
    public DateTime GetCurrentTIme()
    {
        return DateTime.Now;
    }

}
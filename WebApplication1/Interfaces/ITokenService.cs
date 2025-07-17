using System;

namespace WebApplication1.Interfaces;

public interface ITokenService
{
    public string CreateToken( string Username, Guid Userid, string Email,int Time );

    public Guid VerifyTokenAndGetId(string Token);
}

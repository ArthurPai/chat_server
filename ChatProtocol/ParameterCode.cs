namespace ChatProtocol
{
    public enum LoginParameterCode
    {
        Name = 1,
        Password,
    }

    public enum LoginResponseCode
    {
        ID = 1,
        Name,
        Token,
        Nickname,
        Ret = 100,
    }

    public enum ChatParameterCode
    {
        NickName = 1,
        Message,
        Token,
    }

}

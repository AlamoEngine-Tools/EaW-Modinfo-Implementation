namespace EawModinfo.File;

internal interface IModFileNameValidator
{
    public bool Validate(string fileName, out string error);
}
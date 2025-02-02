namespace AET.Modinfo.File;

internal interface IModinfoFileNameValidator
{
    public bool Validate(string fileName, out string error);
}
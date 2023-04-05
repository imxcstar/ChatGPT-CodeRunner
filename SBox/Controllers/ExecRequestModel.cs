namespace SBox.Controllers
{
    /// <summary>
    /// python code info (json format)
    /// </summary>
    public class ExecPythonRequestModel
    {
        /// <summary>
        /// Code to be executed (Note: JSON encoding is required)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// (Optional) List of third-party packages that need to be installed separately, with content format example: numpy=1.19.1
        /// </summary>
        public List<string>? Packages { get; set; }
    }

    /// <summary>
    /// csharp code info (json format)
    /// </summary>
    public class ExecCSharpRequestModel
    {
        /// <summary>
        /// Code to be executed (Note: JSON encoding is required)
        /// </summary>
        public string Code { get; set; }
    }
}

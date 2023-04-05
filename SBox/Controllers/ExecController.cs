using Docker.DotNet.Models;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SBox.Controllers
{
    /// <summary>
    /// Ö´ÐÐ¿ØÖÆÆ÷
    /// </summary>
    [ApiController]
    [Route("exec")]
    public class ExecController : ControllerBase
    {
        public ExecController()
        {
        }

        /// <summary>
        /// Run csharp code in dotnet-script mode. (Note: The executed code needs to be output to the console to have a return.)
        /// </summary>
        /// <param name="execCSharpRequestModel">The requested code content</param>
        /// <returns>result</returns>
        [Route("csharp", Name = "exec_csharp_code")]
        [HttpPost]
        public async Task<string> ExecCSharp([FromBody] ExecCSharpRequestModel execCSharpRequestModel)
        {
            try
            {
                var dockerClient = new DockerClientConfiguration().CreateClient();

                string code = execCSharpRequestModel.Code;
                var codeFile = $"{Path.GetTempFileName()}.csx";
                System.IO.File.WriteAllText(codeFile, code);

                var containerConfig = new CreateContainerParameters
                {
                    Image = "dotnet-script:latest",
                    Tty = true,
                    Cmd = new[] { "/app/test.csx" },
                    HostConfig = new HostConfig
                    {
                        Binds = new[] { $"{codeFile}:/app/test.csx" }
                    }
                };

                var container = await dockerClient.Containers.CreateContainerAsync(containerConfig);

                await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

                var logsStream = await dockerClient.Containers.GetContainerLogsAsync(container.ID, new ContainerLogsParameters { Follow = true, ShowStdout = true });

                var ret = "complete!";
                using (var streamReader = new StreamReader(logsStream, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        ret = await streamReader.ReadToEndAsync();
                    }
                }

                await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { Force = true });

                dockerClient.Dispose();

                System.IO.File.Delete(codeFile);

                return ret;
            }
            catch (Exception)
            {
                return "exec error";
            }
        }

        /// <summary>
        /// Running Python code (Note: The executed code needs to be output to the console to have a return.)
        /// </summary>
        /// <param name="execPythonRequestModel">The requested code and package content</param>
        /// <returns>result</returns>
        [Route("python", Name = "exec_python_code")]
        [HttpPost]
        public async Task<string> ExecPython([FromBody] ExecPythonRequestModel execPythonRequestModel)
        {
            try
            {
                var dockerClient = new DockerClientConfiguration().CreateClient();

                var pythonCode = execPythonRequestModel.Code;
                var pythonCodeBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(pythonCode));


                var cmd = new[] { "python", "-c", $"import base64; exec(base64.b64decode('{pythonCodeBase64}').decode())" };
                if (execPythonRequestModel.Packages != null && execPythonRequestModel.Packages.Any())
                {
                    var ps = string.Join(" ", execPythonRequestModel.Packages);
                    cmd = new[] { "pip", "install", ps, "&&", "python", "-c", $"import base64; exec(base64.b64decode('{pythonCodeBase64}').decode())" };
                }

                var containerConfig = new CreateContainerParameters
                {
                    Image = "python:3.8",
                    Cmd = cmd,
                    Tty = true,
                };

                var container = await dockerClient.Containers.CreateContainerAsync(containerConfig);

                await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

                var logsStream = await dockerClient.Containers.GetContainerLogsAsync(container.ID, new ContainerLogsParameters { Follow = true, ShowStdout = true });

                var ret = "complete!";
                using (var streamReader = new StreamReader(logsStream, Encoding.UTF8))
                {
                    while (!streamReader.EndOfStream)
                    {
                        ret = await streamReader.ReadToEndAsync();
                    }
                }

                await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { Force = true });

                dockerClient.Dispose();

                return ret;
            }
            catch (Exception)
            {
                return "exec error";
            }
        }
    }
}
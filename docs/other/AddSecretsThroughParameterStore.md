# Manage Parameter store

All the sensible data like passwords or credentials of this project have been stored in [AWS Systems Manager Parameter Store](https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-parameter-store.html)

We are using the NuGet extension [AWS .NET Configuration Extension for Systems Manager](https://github.com/aws/aws-dotnet-extensions-configuration) to inject the sensible data stored in the parameter store in the *configuration builder time*. In the same way, we obtain the same behaviour as when is stored in the appsettings.json.

## Where is used?

The NuGet extension is used inside the Shared project. All the projects are calling this to load the appsettings.json. We are using customized appsettings. for each subdomain and environment.

Taking advantage of this and following the same hierarchy we inject the parameter store as an external source, there we are storing all the sensible data as a key values.

### **ConfigurationServiceExtension.cs**

```csharp
public static IConfiguration GetConfiguration(string directory, string projectName, string environment)
{   
    return new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddSystemsManager($"/{projectName}/{environment}")
                .AddEnvironmentVariables()
                .Build();
}
```

Now we are getting data from appsettings local file and Parameter store from AWS.

## How to keep updating keys/values?

To keep update or create new ones we can use the following command [ssm put-parameter](https://docs.aws.amazon.com/cli/latest/reference/ssm/put-parameter.html)

### To create a new parameter

```bash
aws ssm put-parameter  \ 
        --name "/{namespaceOfProject}/{configFeature}/Password" \
        --type "SecureString"  \
        --value "XXXXXXXXXXX"  \
```

### To update a parameter

```bash
aws ssm put-parameter  \ 
        --name "/{namespaceOfProject}/{configFeature}/Password" \
        --type "SecureString"  \
        --value "XXXXXXXXXXX"  \
        --overwrite
```
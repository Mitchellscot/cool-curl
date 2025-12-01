# CoolCurl

A .NET command-line HTTP client designed to simplify API development by generating curl commands and optionally executing HTTP requests with response output saving for debugging.

## Purpose

CoolCurl streamlines API development workflows by:
- Generating properly formatted curl commands from stored configuration
- Managing authentication, headers, and query parameters in a persistent configuration file
- Optionally executing HTTP requests directly and saving responses for debugging
- Providing an easy-to-use CLI for quick API testing and documentation

## Features

### Configuration Management
- **Persistent Settings**: Configuration stored in `~/.cool-curl/.config`
- **Base URL Management**: Set and modify your API base URL
- **Authentication Support**: 
  - None
  - Bearer Token
  - Basic Auth
  - JWT Bearer
- **Request Options**:
  - Default HTTP method (GET, POST, PUT, DELETE, etc.)
  - Follow redirects
  - Custom timeout values
  - Default headers
  - Query parameters
- **Display Preferences**:
  - Toggle progress display
  - Toggle error messages
  - Toggle header display

### Command-Line Features
- **Curl Generation**: Generate curl commands from configuration
- **HTTP Execution**: Execute requests directly with .NET HttpClient
- **File Saving**:
  - Save generated curl commands to `~/.cool-curl/curl-commands/`
  - Save HTTP responses to `~/.cool-curl/responses/`
- **Path Management**: Track recent API paths for quick reuse
- **Interactive Configuration**: Guided setup for all settings

## Command-Line Options

### Basic Operations
```
-h,  --help              Show help message
-v,  --version           Show version information
-c,  --config            Display current settings
-r,  --reset             Delete configuration and start over
-p,  --path <path>       Add a path to recent paths
-e,  --execute           Execute the HTTP request
-s,  --save              Save the curl command to disk
-o,  --output            Save the HTTP response to disk (requires -e)
```

### Configuration Change Options
```
-m,  --method            Change the default HTTP method
-u,  --url               Change the base URL
-a,  --auth              Change the authentication type
-t,  --token             Change the auth token
-un, --username          Change the basic auth username
-pw, --password          Change the basic auth password
-f,  --follow            Change follow redirects setting
-sh, --show-headers      Change show headers setting
-se, --show-errors       Change show errors setting
-sp, --show-progress     Change show progress setting
-mt, --max-timeout       Change max timeout in seconds
-dh, --default-headers   Manage default headers
-qp, --query-parameters  Manage query parameters
```

## Building and Running

### Prerequisites
- .NET 9.0 SDK or later

### Build
```bash
# From the root directory
dotnet build cool-curl.sln

# Or from the src directory
cd src
dotnet build
```

### Run
```bash
# From the src directory
dotnet run

# Or run the compiled executable
.\bin\Debug\net9.0\CoolCurl.exe

# With arguments
dotnet run -- -h
dotnet run -- -p /api/users -e
```

### Install Globally (Optional)
```bash
cd src
dotnet pack
dotnet tool install --global --add-source ./bin/Debug CoolCurl
```

## Usage Examples

### First Time Setup
```bash
# Run without arguments to trigger interactive setup
CoolCurl

# Or display help
CoolCurl -h
```

### Generate Curl Command
```bash
# Generate curl for a specific path
CoolCurl -p /api/users

# Generate and save curl command to disk
CoolCurl -p /api/users -s
```

### Execute HTTP Requests
```bash
# Execute request and display response
CoolCurl -p /api/users -e

# Execute request and save response to disk
CoolCurl -p /api/users -e -o

# Execute and save both curl command and response
CoolCurl -p /api/users -e -s -o
```

### Modify Configuration
```bash
# Change base URL
CoolCurl -u

# Change HTTP method
CoolCurl -m

# Change authentication type
CoolCurl -a

# Manage default headers
CoolCurl -dh

# Manage query parameters
CoolCurl -qp

# View current configuration
CoolCurl -c

# Reset configuration
CoolCurl -r
```

## File Structure

```
~/.cool-curl/
├── .config                    # Configuration file (JSON)
├── curl-commands/             # Saved curl commands
│   └── curl_YYYYMMDD_HHmmss.txt
└── responses/                 # Saved HTTP responses
    └── response_YYYYMMDD_HHmmss.txt
```

## Configuration File Format

The configuration is stored as JSON in `~/.cool-curl/.config`:

```json
{
  "BaseUrl": "http://localhost:5000",
  "DefaultMethod": "GET",
  "RecentPaths": [
    "/api/users"
  ],
  "AuthType": 0,
  "AuthToken": null,
  "BasicAuthUsername": null,
  "BasicAuthPassword": null,
  "FollowRedirects": true,
  "ShowProgress": false,
  "ShowError": true,
  "ShowHeaders": true,
  "MaxTimeSeconds": 30,
  "DefaultHeaders": {
    "Accept": "application/json",
    "Content-Type": "application/json"
  },
  "QueryParameters": {}
}
```

## Development

### Project Structure
```
cool-curl/
├── src/
│   ├── Models/
│   │   ├── Settings.cs
│   │   ├── AuthType.cs
│   │   └── HttpMethods.cs
│   ├── Services/
│   │   ├── ArgumentService.cs
│   │   ├── ConfigurationService.cs
│   │   ├── CurlService.cs
│   │   ├── HelpService.cs
│   │   ├── HttpClientService.cs
│   │   └── HttpUtility.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── CoolCurl.csproj
├── cool-curl.sln
└── README.md
```

## License

MIT License - Feel free to use and modify as needed.

## Contributing

Contributions welcome! Please feel free to submit issues or pull requests.

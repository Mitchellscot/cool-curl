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

### AI-Powered Features
- **AI Curl Generation**: Generate curl commands using natural language prompts
  - Supports Google Gemini (gemini-2.0-flash-exp)
  - Supports OpenAI (gpt-4o-mini)
  - Example: `-ai "make a GET request to /users"`
- **AI Error Debugging**: Automatic AI-powered analysis of HTTP errors
  - Suggests fixes and troubleshooting steps
  - Analyzes error context including URL, method, headers, and timeout settings
  - Can be enabled/disabled per configuration

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
-gk, --gemini-key        Change the Google Gemini API key
-ok, --openai-key        Change the OpenAI API key
-ad, --ai-debugging      Enable/disable AI-powered error debugging
```

### AI Features
```
-ai, --ai-prompt <prompt>  Generate curl command using AI prompt
                           Example: -ai "make a GET request to /users"
                           Supports Google Gemini or OpenAI (gpt-4o-mini)
AI Debugging:              When enabled, HTTP errors are analyzed by AI
                           to suggest fixes and troubleshooting steps
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

### AI Features
```bash
# Generate curl command using AI prompt
CoolCurl -ai "make a GET request to /users"
CoolCurl -ai "POST to /api/products with JSON body"
CoolCurl -ai "delete user with id 123"

# Set up Google Gemini API key
CoolCurl -gk

# Set up OpenAI API key
CoolCurl -ok

# Enable AI debugging for HTTP errors
CoolCurl -ad

# Execute request with AI debugging enabled
CoolCurl -p /api/users -e
# (If request fails, AI will analyze and suggest fixes)
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
    "Content-Type": "application/json",
    "User-Agent": "CoolCurl/1.0"
  },
  "QueryParameters": {},
  "GeminiApiKey": null,
  "OpenAiApiKey": null,
  "AllowAiDebugging": false
}
```

### AuthType Values
- `0` = None
- `1` = BearerToken
- `2` = BasicAuth
- `3` = JwtBearer

## Development

### Project Structure
```
cool-curl/
├── src/
│   ├── infrastructure/
│   │   ├── AiDebugService.cs
│   │   ├── GeminiService.cs
│   │   ├── HttpClientService.cs
│   │   ├── HttpUtility.cs
│   │   ├── IAiClient.cs
│   │   └── OpenAiService.cs
│   ├── models/
│   │   ├── AuthType.cs
│   │   ├── GeminiModels.cs
│   │   ├── HttpMethods.cs
│   │   ├── OpenAiModels.cs
│   │   └── Settings.cs
│   ├── services/
│   │   ├── ApplicationOrchestrator.cs
│   │   ├── ArgumentService.cs
│   │   ├── ConfigurationService.cs
│   │   ├── ConsoleWriter.cs
│   │   ├── CurlService.cs
│   │   ├── HelpService.cs
│   │   └── IConsoleWriter.cs
│   ├── appsettings.json
│   ├── CoolCurl.csproj
│   └── Program.cs
├── tests/
│   ├── CoolCurl.Tests/           # Unit tests
│   │   ├── ApplicationOrchestratorTests.cs
│   │   ├── AuthTypeTests.cs
│   │   ├── ConsoleWriterTests.cs
│   │   ├── CurlServiceTests.cs
│   │   ├── GeminiModelsTests.cs
│   │   ├── HttpMethodsTests.cs
│   │   ├── OpenAiModelsTests.cs
│   │   ├── SettingsTests.cs
│   │   └── CoolCurl.Tests.csproj
│   └── CoolCurl.IntegrationTests/ # Integration tests
│       ├── AiDebugServiceTests.cs
│       ├── HttpClientServiceTests.cs
│       ├── HttpUtilityTests.cs
│       └── CoolCurl.IntegrationTests.csproj
├── cool-curl.sln
└── README.md
```

### Running Tests

**Run all tests:**
```bash
# Unit tests only
dotnet test tests/CoolCurl.Tests/CoolCurl.Tests.csproj

# Integration tests only
dotnet test tests/CoolCurl.IntegrationTests/CoolCurl.IntegrationTests.csproj

# All tests in solution
dotnet test cool-curl.sln
```

**Test coverage:**
```bash
# Generate coverage for unit tests
dotnet test tests/CoolCurl.Tests/CoolCurl.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generate coverage for integration tests
dotnet test tests/CoolCurl.IntegrationTests/CoolCurl.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Generate combined HTML coverage report
reportgenerator "-reports:tests/CoolCurl.Tests/TestResults/coverage.opencover.xml;tests/CoolCurl.IntegrationTests/coverage.opencover.xml" -targetdir:coveragereport-combined -reporttypes:Html

# View coverage report
start coveragereport-combined/index.html
```

## License

MIT License - Feel free to use and modify as needed.

## Contributing

Contributions welcome! Please feel free to submit issues or pull requests.

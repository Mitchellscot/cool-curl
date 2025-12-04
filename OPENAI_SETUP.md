# Getting an OpenAI API Key

CoolCurl now supports **OpenAI's gpt-4o-mini** model for AI-powered curl command generation and HTTP error debugging.

## Why gpt-4o-mini?

`gpt-4o-mini` is OpenAI's most cost-effective model:
- **Cheapest option**: Significantly lower cost than GPT-4 or GPT-3.5-turbo
- **Fast**: Quick response times for curl generation and debugging
- **Capable**: More than sufficient for our use cases

## How to Get an OpenAI API Key

1. **Go to OpenAI Platform**
   - Visit: https://platform.openai.com/

2. **Sign Up or Log In**
   - Create an account or log in if you already have one
   - You may need to verify your email and phone number

3. **Navigate to API Keys**
   - Click on your profile icon (top right)
   - Select "API keys" from the menu
   - Or go directly to: https://platform.openai.com/api-keys

4. **Create a New API Key**
   - Click "Create new secret key"
   - Give it a name (e.g., "CoolCurl")
   - Copy the key immediately (you won't be able to see it again!)

5. **Add to CoolCurl**
   - During initial setup, choose option 2 (OpenAI)
   - Or run: `coolcurl -ok` and paste your key

## Pricing

OpenAI operates on a pay-as-you-go model:
- **gpt-4o-mini** costs approximately:
  - **Input**: $0.15 per 1M tokens
  - **Output**: $0.60 per 1M tokens
- Typical curl generation uses ~500-1000 tokens total
- **Cost per request**: Less than $0.001 (under 1 cent)

## Free Credits

New OpenAI accounts typically receive:
- **$5 in free credits** (as of 2024)
- Expires after 3 months
- Enough for thousands of requests

## Usage Limits

- **Rate limits** apply based on your tier
- Free tier: Usually 3 requests per minute
- Monitor usage at: https://platform.openai.com/usage

## Comparison: Gemini vs OpenAI

| Feature | Google Gemini | OpenAI (gpt-4o-mini) |
|---------|---------------|----------------------|
| **Free tier** | Yes (limited) | $5 credit for new users |
| **Cost** | Free tier available | ~$0.001 per request |
| **Speed** | Fast | Fast |
| **Quality** | Excellent | Excellent |
| **Rate limits** | Varies | 3 req/min (free tier) |

## Recommendations

- **Start with Gemini** if you want completely free
- **Use OpenAI** if you've exhausted Gemini free tier or prefer OpenAI's models
- CoolCurl automatically uses whichever key you configure

## Security Note

⚠️ **Important**: API keys are stored in plain text in `~/.cool-curl/.config`

- Keep your config file secure
- Don't commit it to version control
- Rotate keys periodically

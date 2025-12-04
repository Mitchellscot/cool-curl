# Getting a Google Gemini API Key

CoolCurl supports **Google Gemini AI** (gemini-2.0-flash-exp model) for AI-powered curl command generation and HTTP error debugging.

## Why Gemini?

Google's Gemini API offers excellent value:
- **Free tier available**: Great for getting started
- **Fast**: Quick response times for curl generation and debugging
- **Powerful**: Uses the latest Gemini 2.0 Flash model
- **Generous limits**: 1,500 requests per day on free tier

## How to Get a Gemini API Key

1. **Go to Google AI Studio**
   - Visit: https://aistudio.google.com/

2. **Sign In with Google**
   - Use your Google account to sign in
   - Accept the terms of service

3. **Get API Key**
   - Click "Get API key" in the left sidebar
   - Or go directly to: https://aistudio.google.com/app/apikey
   - Click "Create API key"

4. **Choose or Create a Google Cloud Project**
   - Select an existing project or create a new one
   - The API key will be generated instantly

5. **Copy Your API Key**
   - Copy the key immediately
   - Store it securely (you can view it again later in AI Studio)

6. **Add to CoolCurl**
   - During initial setup, choose option 1 (Gemini)
   - Or run: `coolcurl -gk` and paste your key

## Pricing

Gemini offers a **free tier** with generous limits:

### Free Tier
- **15 requests per minute (RPM)**
- **1,500 requests per day (RPD)**
- **1 million tokens per minute (TPM)**
- No credit card required
- Perfect for personal projects and testing

### Paid Tier (if you exceed free limits)
- **gemini-2.0-flash-exp**:
  - **Input**: $0.075 per 1M tokens
  - **Output**: $0.30 per 1M tokens
- Typical curl generation uses ~500-1000 tokens total
- **Cost per request**: Less than $0.0005 (half a cent)

## Usage Limits

Monitor your usage at:
- https://aistudio.google.com/app/apikey
- View requests per day
- Check rate limit status

## Rate Limits

If you hit rate limits:
- **429 errors** indicate you've exceeded RPM/RPD limits
- Wait a minute and try again
- Free tier: 15 requests per minute
- Paid tier: Higher limits available

## Comparison: Gemini vs OpenAI

| Feature | Google Gemini | OpenAI (gpt-4o-mini) |
|---------|---------------|----------------------|
| **Free tier** | ✅ Yes (1,500 req/day) | $5 credit for new users |
| **Cost (paid)** | ~$0.0005 per request | ~$0.001 per request |
| **Speed** | Fast | Fast |
| **Quality** | Excellent | Excellent |
| **Rate limits (free)** | 15/min, 1,500/day | 3/min (varies) |
| **Setup** | Simple, no credit card | Requires payment method |

## Recommendations

### Use Gemini if:
- ✅ You want a **completely free** option
- ✅ You need **1,500 requests per day**
- ✅ You prefer **no credit card** requirement
- ✅ You want the **latest Gemini 2.0** model

### Use OpenAI if:
- You've exhausted Gemini's free tier
- You prefer OpenAI's models
- You need higher rate limits

## Security Note

⚠️ **Important**: API keys are stored in plain text in `~/.cool-curl/.config`

- Keep your config file secure
- Don't commit it to version control
- Don't share your API key publicly
- Rotate keys periodically
- Use Google Cloud Console to manage/revoke keys

## Managing Your API Key

### View Your Keys
- Go to https://aistudio.google.com/app/apikey
- See all your API keys
- Check usage statistics

### Restrict Your API Key (Recommended)
1. Go to [Google Cloud Console](https://console.cloud.google.com/apis/credentials)
2. Find your API key
3. Click "Edit"
4. Add restrictions:
   - **Application restrictions**: Set to "None" or restrict by IP
   - **API restrictions**: Restrict to "Generative Language API"
5. Save changes

### Revoke a Compromised Key
1. Go to https://aistudio.google.com/app/apikey
2. Click the delete icon next to the key
3. Create a new key
4. Update CoolCurl with `coolcurl -gk`

## Troubleshooting

### "403 Permission Denied"
- Ensure the Generative Language API is enabled in your Google Cloud project
- Check that your API key is valid

### "429 Resource Exhausted"
- You've hit the rate limit (15 requests per minute or 1,500 per day)
- Wait a minute or switch to a paid tier

### "Invalid API Key"
- Double-check you copied the entire key
- Make sure there are no extra spaces
- Try creating a new key

## Additional Resources

- **API Documentation**: https://ai.google.dev/docs
- **Pricing Details**: https://ai.google.dev/pricing
- **AI Studio**: https://aistudio.google.com/
- **Cloud Console**: https://console.cloud.google.com/

## Getting Help

If you need assistance:
- Check the [Google AI documentation](https://ai.google.dev/docs)
- Visit [Google Cloud Support](https://cloud.google.com/support)
- File an issue in the CoolCurl repository

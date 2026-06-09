# Gemini API Setup

This project uses Google's Gemini models as the LLM provider.

## Step 1: Create a Gemini API Key

1. Open Google AI Studio:
   https://aistudio.google.com/apikey

2. Sign in with your Google account.

3. Click **Create API Key**.

4. Select an existing Google Cloud project or create a new one.

5. Copy the generated API key.

---

## Step 2: Configure the Environment Variables

Create or update your `.env` file (copy .env.example):

```env
# LLM
LLM_PROVIDER=gemini
LLM_API_KEY=API_KEY_HERE
LLM_MODEL=gemini-2.5-flash
LLM_MODEL_EMBEDDING=gemini-embedding-001
```

Replace `API_KEY_HERE` with the API key generated in Google AI Studio.

Example:

```env
# LLM
LLM_PROVIDER=gemini
LLM_API_KEY=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
LLM_MODEL=gemini-2.5-flash
LLM_MODEL_EMBEDDING=gemini-embedding-001
```

---

## References

* Google AI Studio: https://aistudio.google.com
* Gemini API Documentation: https://ai.google.dev/gemini-api/docs

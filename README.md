# Clinic AI API

Clinic AI is an AI Agent API.

The agent can:

- Answer questions using uploaded knowledge documents
- Decide when external tools should be used
- Execute tool calls automatically
- Combine tool results with LLM reasoning
- Return contextual and structured responses

The API follows an agent-based architecture where the LLM acts as the decision maker and tools provide external capabilities.

## How the Agent Works

The agent follows the workflow below:

1. User sends a message.
2. The Agent forwards the request to LLM.
3. LLM receives the available tools definition.
4. LLM decides whether a tool is required.
5. If a tool call is requested:
   - The API executes the tool.
   - The result is sent back to LLM.
6. LLM generates the final response.
7. The response is returned to the user.

## Technical Features

- Gemini Integration
- Tool Calling
- Short-Term Memory
- Retrieval-Augmented Generation (RAG)
- Embedding-Based Search
- PostgreSQL + pgvector Vector Store
- Top-K Similarity Retrieval

# API Setup

This project uses Google's Gemini models as the LLM provider.

## Step 1: Create a Gemini API Key

1. Open Google AI Studio: https://aistudio.google.com/apikey
2. Sign in with your Google account.
3. Click **Create API Key**.
4. Select an existing Google Cloud project or create a new one.
5. Copy the generated API key.

---

## Step 2: Configure the Environment Variables
 
Create or update your `.env` file (copy .env.example):

```env
# LLM
LLM_API_KEY=API_KEY_HERE
```

Replace `API_KEY_HERE` with the API key generated in Google AI Studio.

Example:

```env
# LLM
LLM_API_KEY=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
```

---

## Step 3: Start the project:
```docker-compose up```


# Using the AI Agent

Once the application is running, you can interact with the Clinic AI Agent.

Endpoint:
POST /api/chat

Use Scalar if running localhost: http://localhost:5000/scalar
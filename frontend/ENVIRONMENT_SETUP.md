# Environment Variables Setup

## Required Environment Variables

Create a `.env` file in the frontend directory with the following content:

```bash
VUE_APP_API_BASE_URL=http://localhost:5011
```

## Configuration

The application now uses environment variables instead of hardcoded localhost URLs. The API configuration is centralized in `src/config/api.js`.

### For Development
```bash
VUE_APP_API_BASE_URL=http://localhost:5011
```

### For Production
```bash
VUE_APP_API_BASE_URL=https://production-api-url.com
```

## Usage

The API configuration is imported and used in components:

```javascript
import { apiConfig } from '../../config/api.js'

// Use endpoints
const response = await fetch(apiConfig.endpoints.benefit)
```

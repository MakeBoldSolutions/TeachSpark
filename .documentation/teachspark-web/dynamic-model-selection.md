# Dynamic Model Selection for TeachSpark

## Overview

TeachSpark now supports dynamic OpenAI model selection, allowing users to choose from different AI models as they become available. This feature provides flexibility, cost optimization, and future-proofing as new models are released.

## Features

### 1. Model Registry

- **Static Model Definitions**: Predefined models with accurate pricing and capabilities
- **Dynamic Discovery**: Capability to fetch new models from OpenAI API (placeholder implementation)
- **Automatic Caching**: Model information is cached for 24 hours to reduce API calls
- **Education Focus**: Models are tagged for educational suitability

### 2. Intelligent Model Recommendation

The system can recommend the best model based on:

- **Worksheet Type**: Different models excel at different content types
- **Budget Constraints**: Respect maximum cost per request
- **Quality vs Speed**: Choose between fast/cheap or high-quality models  
- **Token Requirements**: Ensure the model can handle the content size
- **Special Features**: Support for structured output when needed

### 3. Cost Estimation

- Real-time cost estimation before generation
- Transparent pricing for informed decisions
- Budget tracking and limits

## Available Models

### Education-Recommended Models

| Model | Description | Cost (Input/Output per 1k tokens) | Best For |
|-------|-------------|-----------------------------------|----------|
| **GPT-4o Mini** | Fast and cost-effective | $0.00015 / $0.0006 | Most educational tasks |
| **GPT-4o** | Most capable model | $0.005 / $0.015 | Complex analysis, creative writing |
| **GPT-4 Turbo** | Reliable performance | $0.01 / $0.03 | General purpose worksheets |

### Specialized Models

| Model | Description | Cost (Input/Output per 1k tokens) | Best For |
|-------|-------------|-----------------------------------|----------|
| **o1 Mini** | STEM reasoning | $0.003 / $0.012 | Math, science worksheets |
| **o1 Preview** | Advanced reasoning | $0.015 / $0.06 | Complex problem solving |

## API Endpoints

All model endpoints require a signed-in user. The anonymous `/api/models` controller was removed
in ps-002; the endpoints below are the ones the worksheet generator uses.

### Get Education-Recommended Models

```
GET /api/worksheet-generator/models
```

### Get Model Recommendations for a Worksheet Type

```
GET /api/worksheet-generator/models/recommend?worksheetType=reading-comprehension
```

### Estimate Cost

```
POST /api/worksheet-generator/models/estimate-cost
```

Refreshing the model registry from the provider is not exposed over HTTP; the registry refreshes
itself when its cache expires.

## Integration with Worksheet Generation

### Controller Integration

The `WorksheetGeneratorController` now includes model selection:

- Available models are loaded in `PrepareViewData()`
- Fallback to default model if registry fails
- Model information displayed with pricing

### Service Integration

The `OpenAILlmService` uses the model registry to:

- Select optimal models based on request criteria
- Apply user preferences when specified
- Estimate costs before generation
- Cache responses efficiently

## Configuration

### appsettings.json

```json
{
  "LlmConfiguration": {
    "Provider": "OpenAI",
    "ApiKey": "your-openai-api-key",
    "DefaultModel": "gpt-4o-mini",
    "MaxTokens": 4000,
    "Temperature": 0.7,
    "MaxCostPerRequest": 1.00,
    "EnableCaching": true,
    "CacheExpirationMinutes": 60
  }
}
```

## Model Selection Logic

### Automatic Recommendation

1. **Filter by Requirements**: Remove models that don't meet token/cost/feature requirements
2. **Score Models**: Rate based on education suitability, cost, speed, and quality preferences  
3. **Type-Specific Bonuses**: Favor models that excel at the specific worksheet type
4. **Return Ranked List**: Primary recommendation plus alternatives

### User Override

Users can manually select any available model, with cost estimation provided upfront.

## Future Enhancements

### Planned Features

- **Live Model Discovery**: Real-time fetching of new models from OpenAI API
- **Usage Analytics**: Track which models perform best for different content types
- **Custom Model Profiles**: Save preferred model configurations
- **Batch Processing**: Optimize model selection for multiple worksheets
- **A/B Testing**: Compare model performance on the same content

### Extensibility

The architecture supports:

- Additional AI providers (Anthropic, Azure OpenAI, etc.)
- Custom model evaluation criteria
- Advanced cost optimization strategies
- Integration with external model registries

## Testing

Run the test script in browser console:

```javascript
// Load the test script
window.TeachSparkModelTests.runAllTests();
```

This will test:

- Model listing
- Recommendation engine  
- Cost estimation
- Registry refresh

## Best Practices

### For Educators

- **Start with GPT-4o Mini**: Best balance of cost and quality for most tasks
- **Use o1 Mini for STEM**: Better mathematical and logical reasoning
- **Choose GPT-4o for Complex Tasks**: Literary analysis, creative writing
- **Monitor Costs**: Use cost estimation before generation

### For Developers

- **Always Handle Fallbacks**: Registry might fail, have static defaults
- **Cache Aggressively**: Model information doesn't change frequently
- **Validate User Selections**: Ensure selected models meet requirements
- **Log Model Performance**: Track success rates and quality metrics

## Troubleshooting

### Common Issues

**Model Registry Fails**

- Falls back to static model definitions
- Check OpenAI API connectivity
- Verify API key permissions

**High Costs**

- Review model selection criteria
- Consider using Mini variants for simple tasks
- Implement daily/monthly budget limits

**Poor Quality Output**

- Try higher-tier models (GPT-4o vs Mini)
- Adjust temperature settings
- Consider specialized models for specific domains

## Architecture

The dynamic model selection system consists of:

1. **`IModelRegistryService`**: Interface for model discovery and recommendation
2. **`OpenAIModelRegistryService`**: Implementation with static + dynamic models
3. **`WorksheetGeneratorApiController`**: Signed-in REST API for model operations
4. **`WorksheetGeneratorController`**: Integration with worksheet creation
5. **`OpenAILlmService`**: Uses registry for intelligent model selection

This architecture provides flexibility to add new providers, models, and selection criteria as the AI landscape evolves.

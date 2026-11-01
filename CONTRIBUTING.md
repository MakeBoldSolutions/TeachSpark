# Contributing to TeachSpark

Thank you for your interest in contributing to TeachSpark! We welcome contributions from developers of all skill levels.

## 🚀 Getting Started

### Prerequisites

Before contributing, ensure you have:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [Node.js](https://nodejs.org/) 18.x or later
- [Git](https://git-scm.com/) for version control
- A GitHub account

### Development Setup

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:

   ```bash
   git clone https://github.com/YOUR_USERNAME/TeachSpark.git
   cd TeachSpark
   ```

3. **Add the upstream repository**:

   ```bash
   git remote add upstream https://github.com/MakeBoldSolutions/TeachSpark.git
   ```

4. **Install dependencies**:

   ```bash
   npm install
   cd TeachSpark.Web
   npm install
   ```

5. **Verify the setup**:

   ```bash
   npm run build
   npm run quality
   ```

## 🔧 Development Workflow

### Creating a Feature Branch

1. **Sync with upstream**:

   ```bash
   git checkout main
   git pull upstream main
   ```

2. **Create a feature branch**:

   ```bash
   git checkout -b feature/your-feature-name
   ```

### Making Changes

1. **Follow our coding standards** (enforced by pre-commit hooks)
2. **Write clear, descriptive commit messages**
3. **Keep commits focused** - one logical change per commit
4. **Test your changes** thoroughly

### Code Quality Requirements

All code must pass our quality checks:

```bash
# Run all quality checks
npm run quality

# Auto-fix issues where possible
npm run quality:fix

# Individual checks
npm run lint        # JavaScript linting
npm run lint:css    # SCSS/CSS linting
npm run format      # Code formatting
```

### Submitting Changes

1. **Push your branch**:

   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create a Pull Request** on GitHub
3. **Describe your changes** clearly in the PR description
4. **Link any related issues**

## 📋 Issue Guidelines

### Before Creating an Issue

- Search existing issues to avoid duplicates
- Check if the issue exists in the latest version
- Provide clear, detailed information

### Bug Reports

When reporting bugs, include:

- **Environment details** (OS, .NET version, Node.js version)
- **Steps to reproduce** the issue
- **Expected behavior** vs **actual behavior**
- **Screenshots or error messages** if applicable
- **Minimal reproduction case** if possible

### Feature Requests

For feature requests, provide:

- **Clear description** of the proposed feature
- **Use case** - why is this feature needed?
- **Acceptance criteria** - what defines success?
- **Implementation suggestions** (if you have any)

## 🎯 Code Standards

### JavaScript/TypeScript

- Use ES6+ features
- Follow ESLint configuration
- Prefer const/let over var
- Use meaningful variable names
- Add JSDoc comments for functions

### SCSS/CSS

- Follow BEM methodology where applicable
- Use Bootstrap utilities when possible
- Maintain consistent indentation
- Follow Stylelint configuration

### C# (.NET)

- Follow Microsoft's C# coding conventions
- Use meaningful names for classes, methods, and variables
- Add XML documentation comments for public APIs
- Follow SOLID principles

### Git Commit Messages

Use the conventional commit format:

```
type(scope): description

[optional body]

[optional footer]
```

Types:

- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

Examples:

```
feat(auth): add user authentication system
fix(webpack): resolve bundle size optimization
docs(readme): update installation instructions
```

## 🧪 Testing

### Frontend Testing

- Ensure webpack builds successfully
- Test in multiple browsers
- Verify responsive design
- Check accessibility compliance

### Backend Testing

- Write unit tests for new functionality
- Ensure integration tests pass
- Test API endpoints thoroughly
- Verify database migrations

## 📦 Release Process

### Version Numbering

We follow [Semantic Versioning](https://semver.org/):

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Release Checklist

- [ ] All tests pass
- [ ] Documentation updated
- [ ] Version bumped appropriately
- [ ] Changelog updated
- [ ] Security review completed

## 🎖️ Recognition

Contributors will be recognized in:

- GitHub contributors list
- Release notes for significant contributions
- Special thanks in project documentation

## 📞 Getting Help

- **Questions**: [Start a discussion](https://github.com/MakeBoldSolutions/TeachSpark/discussions)
- **Issues**: [Create an issue](https://github.com/MakeBoldSolutions/TeachSpark/issues)
- **Real-time chat**: Check project discussions for community links

## 📝 License

By contributing to TeachSpark, you agree that your contributions will be licensed under the same [MIT License](LICENSE) that covers the project.

---

Thank you for contributing to TeachSpark! 🎓✨

# 🎓 TeachSpark

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Node.js](https://img.shields.io/badge/Node.js-18%2B-green.svg)](https://nodejs.org/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)

**Live Site**: [teach.makeboldspark.com](https://teach.makeboldspark.com)

> **LLM-Powered Educational Website** - A modern, interactive learning platform that harnesses the power of Large Language Models to create personalized educational experiences.

## About

TeachSpark is a live demonstration of an AI-powered educational platform built with .NET 10 MVC. It showcases how Large Language Models can be integrated into a modern web application to deliver personalized, adaptive learning experiences.

**Live Site**: [teach.makeboldspark.com](https://teach.makeboldspark.com)

> Built by [Mark Hazleton](https://markhazleton.com) — Technical Solutions Architect
> Part of the [MakeBoldSpark](https://makeboldspark.com) portfolio of technical demonstrations.

## 🌟 Overview

TeachSpark is a cutting-edge educational platform built with .NET 10 MVC and powered by advanced Large Language Models. It provides an intelligent, adaptive learning environment that personalizes content delivery based on individual learning patterns and preferences.

### ✨ Key Features

- 🤖 **AI-Powered Learning** - Intelligent content adaptation using LLM technology
- 📚 **Interactive Curriculum** - Dynamic course content with real-time feedback
- 🎯 **Personalized Pathways** - Customized learning journeys for each student
- 📊 **Progress Analytics** - Comprehensive learning analytics and insights
- 🌐 **Modern Web Experience** - Responsive design with world-class UX
- ⚡ **High Performance** - Optimized frontend build system with webpack

## 🚀 Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later (the repo pins SDK 10.0.101 via `global.json`)
- [Node.js](https://nodejs.org/) 18.x or later
- [Git](https://git-scm.com/) for version control

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/MakeBoldSolutions/TeachSpark.git
   cd TeachSpark
   ```

2. **Install dependencies**

   ```bash
   # Install root-level dependencies (Husky, lint-staged)
   npm install
   
   # Install frontend dependencies
   cd TeachSpark.Web
   npm install
   ```

3. **Build the application**

   ```bash
   # From TeachSpark.Web directory
   npm run build
   
   # Or build the .NET solution (automatically runs npm build)
   dotnet build TeachSpark.sln
   ```

4. **Run the application**

   ```bash
   # Development mode with hot reloading
   npm run dev
   
   # Or run the .NET application
   dotnet run
   ```

5. **Open your browser**
   Navigate to `http://localhost:5000` (or the port specified in your configuration)

## 🏗️ Architecture

TeachSpark is built with a modern, scalable architecture:

### Backend

- **Framework**: .NET 10 MVC
- **Language**: C#
- **Architecture**: Clean Architecture principles
- **Database**: Entity Framework Core (configurable provider)

### Frontend

- **Build System**: Webpack 5 with modern optimization
- **Styling**: Bootstrap 5 through the repository's webpack-managed SCSS pipeline
- **JavaScript**: ES6+ with Babel compilation
- **Quality Tools**: ESLint, Prettier, Stylelint
- **Asset Management**: Automated bundling and optimization

### Development Tools

- **Git Hooks**: Husky for pre-commit automation
- **Code Quality**: Automated linting and formatting
- **Bundle Analysis**: Webpack Bundle Analyzer
- **Hot Reloading**: Development server with live updates

## 📁 Project Structure

```
TeachSpark/
├── 📁 .husky/                    # Git hooks configuration
├── 📁 .vscode/                   # VS Code settings
├── 📁 TeachSpark.Web/            # Main web application
│   ├── 📁 Controllers/           # MVC Controllers
│   ├── 📁 Models/                # Data models and view models
│   ├── 📁 Views/                 # Razor views
│   ├── 📁 src/                   # Frontend source files
│   │   ├── 📁 js/                # JavaScript modules
│   │   ├── 📁 scss/              # SCSS stylesheets
│   │   └── 📄 favicon.ico        # Static assets
│   ├── 📁 wwwroot/               # Generated static files
│   ├── 📄 webpack.config.js      # Webpack configuration
│   ├── 📄 package.json           # Frontend dependencies
│   └── 📄 *.config.js            # Build tool configurations
├── 📄 TeachSpark.sln             # Visual Studio solution
├── 📄 README.md                  # This file
├── 📄 LICENSE                    # MIT License
└── 📄 package.json               # Root-level dependencies
```

## 🛠️ Development

### Available Scripts

From the `TeachSpark.Web` directory:

```bash
# Development
npm run dev              # Start development server with hot reloading
npm run watch            # Build in watch mode

# Building
npm run build            # Production build
npm run build:dev        # Development build
npm run build:analyze    # Build with bundle analysis

# Code Quality
npm run lint             # Run ESLint
npm run lint:fix         # Fix ESLint issues
npm run lint:css         # Run Stylelint
npm run lint:css:fix     # Fix Stylelint issues
npm run format           # Format code with Prettier
npm run format:check     # Check code formatting
npm run quality          # Run all quality checks
npm run quality:fix      # Fix all auto-fixable issues

# Maintenance
npm run clean            # Clean build artifacts
npm run clean:all        # Clean everything and recreate directories
```

### Code Quality Standards

This project maintains high code quality standards with automated tooling:

- **ESLint**: JavaScript linting with modern ES6+ rules
- **Prettier**: Consistent code formatting
- **Stylelint**: SCSS/CSS linting and best practices
- **Pre-commit hooks**: Repository-defined pre-commit automation
- **Husky + lint-staged**: Only lint changed files for faster commits

### Build System

TeachSpark uses a modern webpack-based build system that provides:

- 🔄 **Hot Module Replacement** for instant development feedback
- 📦 **Code Splitting** for optimal loading performance
- 🗜️ **Asset Optimization** with minification and compression
- 🔗 **Content Hashing** for cache invalidation
- 📊 **Bundle Analysis** for performance monitoring

For detailed build system documentation, see [.documentation/teachspark-web/build-system-documentation.md](.documentation/teachspark-web/build-system-documentation.md).

Project documentation that should not live beside code is organized under [.documentation/guide.md](.documentation/guide.md).

## 🤝 Contributing

We welcome contributions from the community! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details on how to get started.

### How to Contribute

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Make your changes** following our coding standards
4. **Run quality checks** (`npm run quality`)
5. **Commit your changes** (`git commit -m 'Add amazing feature'`)
6. **Push to the branch** (`git push origin feature/amazing-feature`)
7. **Open a Pull Request**

### Development Guidelines

- Follow the existing code style and conventions
- Write clear, descriptive commit messages
- Add tests for new functionality
- Update documentation for significant changes
- All commits should run the repository's defined quality checks before review

## 📋 Issues and Support

- 🐛 **Bug Reports**: [Create an issue](https://github.com/MakeBoldSolutions/TeachSpark/issues/new?template=bug_report.md)
- 💡 **Feature Requests**: [Create an issue](https://github.com/MakeBoldSolutions/TeachSpark/issues/new?template=feature_request.md)
- ❓ **Questions**: [Start a discussion](https://github.com/MakeBoldSolutions/TeachSpark/discussions)
- 📧 **Contact**: Please use GitHub Issues for all inquiries

## Repo Story

An evidence-based narrative of this repository's development history, contributor patterns, and architecture:
[Repo Story (2026-11-02)](.knowledge/guides/repo-story/repo-story-2026-11-02.md)

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built with [.NET](https://dotnet.microsoft.com/) and [ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/mvc/)
- Frontend powered by [Webpack](https://webpack.js.org/) and [Bootstrap](https://getbootstrap.com/)
- Code quality tools: [ESLint](https://eslint.org/), [Prettier](https://prettier.io/), [Stylelint](https://stylelint.io/)
- Git hooks managed by [Husky](https://typicode.github.io/husky/)

---

<div align="center">

**[⭐ Star this repository](https://github.com/MakeBoldSolutions/TeachSpark/stargazers)** if you find it helpful!

[TeachSpark](https://teach.makeboldspark.com) — built by [Mark Hazleton](https://markhazleton.com) · [MakeBoldSpark](https://makeboldspark.com)

</div>

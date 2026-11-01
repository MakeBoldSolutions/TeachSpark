const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const { WebpackManifestPlugin } = require('webpack-manifest-plugin');

module.exports = {
    mode: 'production',
    entry: {
        // Split vendor libraries from app code
        vendor: ['bootstrap', 'jquery'],
        site: ['./src/scss/site.scss', './src/js/site.js'],
        validation: './src/js/validation.js'
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
        filename: 'js/[name].[contenthash:8].js',
        chunkFilename: 'js/[name].[contenthash:8].chunk.js',
        assetModuleFilename: 'assets/[name].[contenthash:8][ext]',
        clean: {
            keep: /\.gitkeep/,
        },
        publicPath: '/'
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: {
                        presets: [
                            ['@babel/preset-env', {
                                targets: {
                                    browsers: ['> 1%', 'last 2 versions', 'not dead']
                                },
                                modules: false,
                                useBuiltIns: 'usage',
                                corejs: 3
                            }]
                        ],
                        plugins: ['@babel/plugin-syntax-dynamic-import']
                    }
                }
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            importLoaders: 3,
                            sourceMap: false
                        }
                    },                    {
                        loader: 'postcss-loader',
                        options: {
                            postcssOptions: {
                                plugins: [
                                    ['autoprefixer', {
                                        overrideBrowserslist: ['> 1%', 'last 2 versions', 'not dead', 'not ie 11'],
                                        grid: 'autoplace',
                                        flexbox: 'no-2009'
                                    }],
                                    ['postcss-preset-env', {
                                        stage: 2,
                                        features: {
                                            'nesting-rules': true,
                                            'custom-properties': true
                                        },
                                        autoprefixer: false
                                    }]
                                ]
                            },
                            sourceMap: false
                        }
                    },
                    {
                        loader: 'sass-loader',
                        options: {
                            sourceMap: false,
                            sassOptions: {
                                quietDeps: true,
                                outputStyle: 'compressed'
                            }
                        }
                    }
                ]
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            importLoaders: 1,
                            sourceMap: false
                        }                    },
                    {
                        loader: 'postcss-loader',
                        options: {
                            postcssOptions: {
                                plugins: [
                                    ['autoprefixer', {
                                        overrideBrowserslist: ['> 1%', 'last 2 versions', 'not dead', 'not ie 11'],
                                        grid: 'autoplace',
                                        flexbox: 'no-2009'
                                    }]
                                ]
                            },
                            sourceMap: false
                        }
                    }
                ]
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                type: 'asset/resource',
                generator: {
                    filename: 'fonts/[name].[contenthash:8][ext]'
                }
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg)$/,
                type: 'asset',
                parser: {
                    dataUrlCondition: {
                        maxSize: 8 * 1024 // 8kb
                    }
                },
                generator: {
                    filename: 'images/[name].[contenthash:8][ext]'
                }
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: 'css/[name].[contenthash:8].css',
            chunkFilename: 'css/[name].[contenthash:8].chunk.css'
        }),        new CopyWebpackPlugin({
            patterns: [
                {
                    from: 'src/favicon.ico',
                    to: 'favicon.ico'
                }
            ]        }),        new WebpackManifestPlugin({
            fileName: 'assets-manifest.json',
            publicPath: '',
            filter: (file) => {
                // Include JS, CSS, and font files that .NET might reference
                return file.name.endsWith('.js') || 
                       file.name.endsWith('.css') || 
                       file.name.endsWith('.woff') || 
                       file.name.endsWith('.woff2');
            },
            generate(seed, files, entrypoints) {
                const manifest = files.reduce((manifest, file) => {
                    const name = file.name;
                    let logicalName = name;
                    
                    // Create consistent logical names with leading slashes
                    if (name.startsWith('site.') && name.endsWith('.js')) {
                        logicalName = '/js/site.js';
                    } else if (name.startsWith('validation.') && name.endsWith('.js')) {
                        logicalName = '/js/validation.js';
                    } else if (name.startsWith('site.') && name.endsWith('.css')) {
                        logicalName = '/css/site.css';
                    } else if (name.startsWith('vendors.') && name.endsWith('.css')) {
                        logicalName = '/css/vendors.css';
                    } else if (name.startsWith('vendor.') && name.endsWith('.js')) {
                        logicalName = '/js/vendor.js';
                    } else if (name.startsWith('runtime.') && name.endsWith('.js')) {
                        logicalName = '/js/runtime.js';
                    } else if (name.startsWith('bootstrap.') && name.endsWith('.js')) {
                        logicalName = '/js/bootstrap.js';
                    } else if (name.startsWith('jquery.') && name.endsWith('.js')) {
                        logicalName = '/js/jquery.js';
                    } else if (name.includes('bootstrap-icons') && name.endsWith('.woff2')) {
                        logicalName = '/fonts/bootstrap-icons.woff2';
                    } else if (name.includes('bootstrap-icons') && name.endsWith('.woff')) {
                        logicalName = '/fonts/bootstrap-icons.woff';
                    } else {
                        // For any other files, create logical names based on their location
                        const filePath = file.path;
                        if (filePath.includes('/js/')) {
                            logicalName = `/js/${path.basename(name, path.extname(name))}.js`;
                        } else if (filePath.includes('/css/')) {
                            logicalName = `/css/${path.basename(name, path.extname(name))}.css`;
                        } else if (filePath.includes('/fonts/')) {
                            logicalName = `/fonts/${name}`;
                        } else {
                            logicalName = `/${name}`;
                        }
                    }
                    
                    // Ensure the file path starts with a forward slash
                    const filePath = file.path.startsWith('/') ? file.path : '/' + file.path;
                    
                    // Add both the logical name and the actual filename for flexibility
                    manifest[logicalName] = filePath;
                    
                    // Also add without leading slash for backward compatibility
                    const logicalNameNoSlash = logicalName.substring(1);
                    if (logicalNameNoSlash !== logicalName) {
                        manifest[logicalNameNoSlash] = filePath;
                    }
                    
                    // Add the actual filename as well for direct lookups
                    if (name !== logicalName && name !== logicalNameNoSlash) {
                        manifest[name] = filePath;
                    }
                    
                    return manifest;
                }, seed);
                
                return manifest;
            }
        }),
        // Uncomment for bundle analysis
        // new BundleAnalyzerPlugin({
        //     analyzerMode: 'static',
        //     openAnalyzer: false,
        //     reportFilename: 'bundle-report.html'
        // })
    ],
    optimization: {
        minimize: true,
        minimizer: [
            new TerserPlugin({
                terserOptions: {
                    compress: {
                        drop_console: true,
                        drop_debugger: true
                    },
                    mangle: true,
                    format: {
                        comments: false
                    }
                },
                extractComments: false
            }),
            new CssMinimizerPlugin({
                minimizerOptions: {
                    preset: [
                        'default',
                        {
                            discardComments: { removeAll: true },
                            normalizeWhitespace: true,
                            colormin: true,
                            convertValues: true,
                            discardDuplicates: true,
                            discardEmpty: true,
                            mergeRules: true,
                            minifySelectors: true
                        }
                    ]
                }
            })
        ],
        splitChunks: {
            chunks: 'all',
            cacheGroups: {
                vendor: {
                    test: /[\\/]node_modules[\\/]/,
                    name: 'vendors',
                    chunks: 'initial',
                    priority: 10,
                    reuseExistingChunk: true
                },
                bootstrap: {
                    test: /[\\/]node_modules[\\/]bootstrap[\\/]/,
                    name: 'bootstrap',
                    chunks: 'all',
                    priority: 20
                },
                jquery: {
                    test: /[\\/]node_modules[\\/]jquery[\\/]/,
                    name: 'jquery',
                    chunks: 'all',
                    priority: 15
                },
                common: {
                    name: 'common',
                    minChunks: 2,
                    chunks: 'all',
                    priority: 5,
                    reuseExistingChunk: true,
                    enforce: true
                }
            }
        },
        runtimeChunk: {
            name: 'runtime'
        }
    },
    resolve: {
        extensions: ['.js', '.scss', '.css'],
        alias: {
            '@': path.resolve(__dirname, 'src'),
            '@scss': path.resolve(__dirname, 'src/scss'),
            '@js': path.resolve(__dirname, 'src/js')
        }
    },
    performance: {
        maxAssetSize: 350000,
        maxEntrypointSize: 600000,
        hints: 'warning'
    },
    stats: {
        assets: true,
        children: false,
        chunks: false,
        modules: false,
        builtAt: true,
        timings: true
    }
};

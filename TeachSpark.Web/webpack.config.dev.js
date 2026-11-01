const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const { WebpackManifestPlugin } = require('webpack-manifest-plugin');

module.exports = {
    mode: 'development',
    entry: {
        site: ['./src/scss/site.scss', './src/js/site.js'],
        validation: './src/js/validation.js'
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
        filename: 'js/[name].js',
        chunkFilename: 'js/[name].chunk.js',
        assetModuleFilename: 'assets/[name][ext]',
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
                                modules: false
                            }]
                        ]
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
                            sourceMap: true
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
                            sourceMap: true
                        }
                    },
                    {
                        loader: 'sass-loader',
                        options: {
                            sourceMap: true,
                            sassOptions: {
                                quietDeps: true
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
                            sourceMap: true
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
                                    }]
                                ]
                            },
                            sourceMap: true
                        }
                    }
                ]
            },
            {
                test: /\.(woff|woff2|eot|ttf|otf)$/,
                type: 'asset/resource',
                generator: {
                    filename: 'fonts/[name][ext]'
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
                    filename: 'images/[name][ext]'
                }
            }
        ]
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: 'css/[name].css',
            chunkFilename: 'css/[name].chunk.css'
        }),
        new CopyWebpackPlugin({
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
            map: (file) => {
                // Map actual filenames to logical names for easier lookup
                const name = file.name;
                let logicalName = name;
                
                // In development, map actual file names to logical names the .NET app expects
                if (name === 'site.css') {
                    logicalName = 'site.css';
                } else if (name === 'site.js') {
                    logicalName = 'site.js';
                } else if (name === 'validation.js') {
                    logicalName = 'validation.js';
                } else if (name === 'vendors.css') {
                    logicalName = 'vendors.css';
                }
                
                return {
                    ...file,
                    name: logicalName,
                    path: file.path.startsWith('/') ? file.path : '/' + file.path
                };
            }
        })
    ],
    resolve: {
        extensions: ['.js', '.scss', '.css'],
        alias: {
            '@': path.resolve(__dirname, 'src'),
            '@scss': path.resolve(__dirname, 'src/scss'),
            '@js': path.resolve(__dirname, 'src/js')
        }
    },
    devtool: 'eval-source-map',
    devServer: {
        static: {
            directory: path.join(__dirname, 'wwwroot'),
        },
        compress: true,
        port: 3000,
        hot: true,
        open: true,
        watchFiles: ['src/**/*']
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

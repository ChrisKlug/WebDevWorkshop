const path = require('path');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = (env, agrs) => {
    const isProdMode = !!(env && env.prod);
    console.log("Prodmode " + isProdMode);

    return {
        mode: isProdMode ? 'production' : 'development',
        entry: {
            app: './TypeScript/App.ts',
            styles: './Styles/App.scss'
        },
        resolve: {
            extensions: ['.ts', '.js']
        },
        output: {
            filename: '[name].bundle.js',
            path: path.resolve('./wwwroot'),
            devtoolModuleFilenameTemplate: (info) => "/" + info.resourcePath
        },
        module: {
            rules: [
                { test: /\.ts$/, use: 'ts-loader' },
                { test: /\.html$/, use: 'html-loader' },
                {
                    test: /\.scss$/, use: [
                        //'style-loader',
                        {
                            loader: MiniCssExtractPlugin.loader
                        },
                        'css-loader',
                        'sass-loader'
                    ]
                }
            ]
        },
        plugins: [
            new CopyWebpackPlugin([
                { from: 'node_modules/oidc-client/dist/oidc-client.min.js', to: '' }
            ]),
            new webpack.ProvidePlugin({ ko: 'knockout' }),
            new MiniCssExtractPlugin()
        ],
        devtool: isProdMode ? '' : 'source-map'
    }
}
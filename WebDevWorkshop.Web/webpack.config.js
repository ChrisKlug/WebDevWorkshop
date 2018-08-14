const path = require("path");
const copyWebpackPlugin = require("copy-webpack-plugin");

module.exports = env => ({
    mode: (env && env.production) ? 'production' : 'development',
    entry: {
        app: "./SPA/App.ts",
        less: "./Styles/Site.less"
    },
    resolve: {
        extensions: ['.ts', '.js']
    },
    output: {
        filename: "[name].bundle.js",
        path: path.resolve("./wwwroot/scripts"),
        devtoolModuleFilenameTemplate: (info) => "/" + info.resourcePath
    },
    module: {
        rules: [
            { test: /\.ts$/, use: 'ts-loader' },
            { test: /\.less$/, use: [ 
                "style-loader",
                "css-loader",
                "less-loader"
             ] },
            { test: /\.(gif|png)$/, loader: 'url-loader', options: {
                limit: 15000,
                name: "images/[name].[ext]",
                publicPath: "scripts/"
            }}
        ]
    },
    plugins: [
        new copyWebpackPlugin([{from: "node_modules/oidc-client/dist/oidc-client.min.js", to: ""}])
    ],
    devtool: (env && env.production) ? '' : 'source-map',
})
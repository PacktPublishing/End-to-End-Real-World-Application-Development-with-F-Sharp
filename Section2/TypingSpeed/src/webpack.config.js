const path = require("path");
const webpack = require("webpack");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');


var isProduction = !process.argv.find(v => v.indexOf('webpack-dev-server') !== -1);
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");
var commonPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: './src/index.html'
    })

];
module.exports = {
    mode: isProduction ? "production" : "development",
    entry:   "./src/TypingSpeed.fsproj",
    output: {
        path: path.join(__dirname, "../public"),
        filename: isProduction ? '[name].[hash].js' : '[name].js',
        devtoolModuleFilenameTemplate: info =>
            path.resolve(info.absoluteResourcePath).replace(/\\/g, '/'),
    },
    plugins: isProduction ?
        commonPlugins.concat([
            new CopyWebpackPlugin([
                { from: './src/static' }
            ])
        ])
        : commonPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
            new webpack.NamedModulesPlugin()
        ]),
    devtool: isProduction ?  'undefined':'source-map',
    devServer: {
        contentBase: "./src/static",
        port: 8081,
        hot: true,
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }]
    }
}

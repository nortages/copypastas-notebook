const path = require('path');
const webpack = require('webpack');

module.exports = {
    mode: 'development',
    entry: {
        Layout: './wwwroot/src/ts/_Layout.ts',
        homeIndex: {
            import: './wwwroot/src/ts/Home/index.ts',
            dependOn: "Layout"
        },
        editMode: {
            import: './wwwroot/src/ts/Home/edit_mode.ts',
            dependOn: "Layout"
        },
    },
    devtool: 'inline-source-map',
    module: {
        rules: [
            {
                test: /\.css$/i,
                use: ["style-loader", "css-loader"],
            },
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
            // {
            //     test: require.resolve('./wwwroot/src/ts/search.ts'),
            //     use: [
            //         {
            //             loader: 'expose-loader',
            //             options: {
            //                 exposes: {
            //                     globalName: 'searchParams',
            //                     override: true
            //                 }
            //             }
            //         },
            //         {
            //             loader: 'ts-loader'
            //         }
            //     ],
            //     exclude: /node_modules/
            // },
            {
                test: require.resolve('darkreader'),
                loader: 'expose-loader',
                options: {
                    exposes: "DarkReader",
                }
            },
        ],
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    output: {
        filename: '[name].bundle.js',
        path: path.resolve(__dirname, 'wwwroot/dist/js'),
    },        
    plugins: [
        // new webpack.ProvidePlugin({
        //     $: 'jquery',
        //     jQuery: 'jquery',
        //     "window.jQuery": 'jquery',
        //     "window.$": 'jquery',
        // }),
        // new webpack.ProvidePlugin({
        //     : 'search.ts'
        // })
        // new webpack.DefinePlugin({
        //     searchParams: {}
        // })
    ],
    // optimization: {
    //     runtimeChunk: 'single',
    // },
    externals: {
        'jquery': 'jQuery',
        'DarkReader': 'darkreader',
        // 'searchParams': require.resolve('./wwwroot/src/ts/search.ts')
    },
};
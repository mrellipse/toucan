var path = require('path');
var webpack = require('webpack');
var merge = require('webpack-merge');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var WebpackCleanupPlugin = require('webpack-cleanup-plugin');

var config = require('../package.json').config;
var webpackBase = require('./webpack.base.js');

module.exports = merge(webpackBase, {
  devtool: '#eval-source-map',
  output: {
    path: path.resolve(__dirname, '../../server/wwwroot')
  },
  plugins: [
    new webpack.DefinePlugin({
      'process.env.NODE_ENV': '"development"'
    }),

    new HtmlWebpackPlugin({
      inject: 'body',
      template: path.resolve(__dirname, '../src/index.html')
    }),
    new WebpackCleanupPlugin()
  ]
});

const webpack = require("webpack");
const path = require("path");
const fs = require("fs");
const nodeExternals = require("webpack-node-externals");

var config = {
  /*
    mocha-webpack will set entry/output options at runtime so we don't need to set them here
  entry:
  output: */
  target: "node",
  devtool: "cheap-module-eval-source-map",
  /*
    Use webpack-node-externals to prevent bundling anything referenced in node_modules.
    At runtime, these modules can be loaded by Node and therefore do not need to be bundled by Webpack. */
  externals: [
    nodeExternals({
      modulesDir: path.join(__dirname, "node_modules"),
      whitelist: 'vue' // whitelist vue so the runtime & compiler version below is included in the bundle, and templates can be compiled
    })
  ],
  resolve: {
    alias: {
      vue: 'vue/dist/vue.esm.js'
    },
    extensions: [".ts", ".js"]
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        loader: "ts-loader"
      }
    ]
  }
};

module.exports = config;

var path = require('path');
var autoprefixer = require('autoprefixer');
var webpack = require('webpack');

var babelOptions = {
  plugins: ['transform-runtime'],
  presets: [
    'env',
    [
      'es2015',
      {
        'modules': false
      }
    ],
    'es2016'
  ]
};

module.exports = {

  entry: {
    vendor: [
      path.resolve(__dirname, '../node_modules/jquery/dist/jquery.slim.js'),
      path.resolve(__dirname, '../node_modules/tether/dist/js/tether.js'),
      path.resolve(__dirname, '../node_modules/bootstrap/dist/js/bootstrap.js'),
      path.resolve(__dirname, '../node_modules/jwt-decode/lib/index.js'),
    ],
    admin: [path.resolve(__dirname, '../src/admin/admin.ts')],
    app: [path.resolve(__dirname, '../src/root/root.ts')]
  },

  plugins: [
    new webpack.optimize.CommonsChunkPlugin({
      name: 'common',
      minChunks: 2
    }), new webpack.ProvidePlugin({
      $: 'jquery',
      jQuery: 'jquery',
      Tether: 'tether'
    })
  ],

  resolve: {
    alias: {
      'src': path.resolve(__dirname, '../src'),
      'assets': path.resolve(__dirname, '../src/assets'),
      'components': path.resolve(__dirname, '../src/components'),
      'state': path.resolve(__dirname, '../src/state'),
      'vue$': 'vue/dist/vue.js'
    },
    extensions: ['.js', '.ts', '.html'],
    enforceExtension: false,
    modules: [path.join(__dirname, '../node_modules')]
  },

  module: {

    rules: [
      {
        test: /\.html$/,
        use: { loader: 'vue-html-loader' }
      },
      {
        test: /\.ts$/,
        exclude: /node_modules/,
        use: [
          {
            loader: 'babel-loader',
            options: babelOptions
          },
          {
            loader: 'ts-loader'
          }
        ]
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: [
          {
            loader: 'babel-loader',
            options: babelOptions
          }
        ]
      },
      {
        test: /\.(png|jpg|jpeg|gif)$/,
        exclude: /node_modules/,
        use: {
          loader: 'url-loader',
          options: {
            prefix: 'img',
            limit: 5000
          }
        }
      },
      {
        test: /\.scss$/,
        exclude: /node_modules/,
        use: [
          { loader: 'style-loader' },
          { loader: 'css-loader' },
          {
            loader: 'postcss-loader',
            options: {
              plugins: ['postcss-loader']
            }
          },
          {
            loader: 'sass-loader',
            options: { sourceMap: true }
          }
        ]
      },
      {
        test: /\.(ttf|eot|svg)?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
        exclude: /node_modules/,
        use: [{ loader: 'file-loader' }]
      },
      {
        test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
        exclude: /node_modules/,
        use: {
          loader: 'url',
          options: {
            prefix: 'font',
            limit: 5000,
            mimetype: 'application/font-woff'
          }
        }
      }
    ]
  }
}
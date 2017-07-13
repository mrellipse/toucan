
## ./ui/app/\<project folder>

* _assets_ - static resources for inclusion in client bundles
* _common_ - utility classes
* _components_ - store global components in this folder
* _locales_ - language resource files
* _model_ - models used by multiple areas
* _routes_ - vuejs router navigation guards
* _services_ - helper/domain classes
* _store_ - vuex store used by global components
* _styles_ - global SASS styles
* _validation_ - shared validations rules

Other components will be found in specific sub-areas (such as 'root' or 'admin'). These areas have a similar structure to the project root, but at minimum will contain

* _layout_  - contains any custom layouts
* _navigation_  - navbar or menu components for use by layouts
* _routes_  - route configuration, and route names
* _store_ - a custom vuex store that extends the vuex store
* _[area].ts_ - code to bootstrap the entry point
* _[area].html_ - entry point page
* _[area].scss_ - area SASS styles

### Areas * Webpack

There are multiple areas defined in the client VueJs application ... 

* ./ui/app/root/root.ts => https://localhost:5000
* ./ui/app/admin/admin.ts  => https://localhost:5000/admin

The motive behind this decision was to

* optimize end user experience (by ensuring only required resources are downloaded)
* enable switching between layouts in different areas of a site
* establish a common set of components for re-use

To support multiple entry points as described above, the webpack build process is configured to emit multiple files

* vendor.js - contains core third-party dependencies
* common.js - contains code used by all areas/subsites (using [commons-chunk-plugin](http://webpack.github.io/docs/examples.html#commons-chunk-plugin))
* [area].js - contains area-specific code and resources
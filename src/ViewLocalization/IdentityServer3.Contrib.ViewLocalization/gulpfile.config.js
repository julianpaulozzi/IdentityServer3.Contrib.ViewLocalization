'use strict';
var GulpConfig = (function () {
    function GulpConfig() {
        //Got tired of scrolling through all the comments so removed them
        //Don't hurt me AC :-)
        this.source = "./";
        this.sourceApp = this.source + "App/";
        this.allAppHTML = this.sourceApp + "/**/*.html";
        this.allAppJS = [
            this.sourceApp + "app.js",
            this.sourceApp + "**/*.js"
        ];
        this.allAppCss = this.sourceApp + "/**/*.css";
        this.allAppLess = this.sourceApp + "/**/*.less";

        this.jsOutputPath = this.source + "/wwwroot/scripts";
        this.cssOutputPath = this.source + "/wwwroot/content";
        this.allJSOutput = [this.source + "/wwwroot/scripts/**/*.js"];

        this.mainJsFile = 'scripts.js';
        this.mainCssFile = 'styles.css';
        
        this.vendorJSFiles = [
            "bower_components/jquery/dist/jquery.min.js",
            "bower_components/bootstrap/dist/js/bootstrap.min.js",
            "app_components/htmlencode/encoder.min.js",
            "bower_components/angular/angular.min.js",
            "bower_components/angular-sanitize/angular-sanitize.min.js",
            "bower_components/angular-cookies/angular-cookies.min.js",
            "bower_components/angular-translate/angular-translate.min.js",
            "bower_components/angular-translate-storage-cookie/angular-translate-storage-cookie.min.js",
            "bower_components/angular-translate-loader-url/angular-translate-loader-url.min.js"
        ];

        this.vendorCSSFiles = [
            "bower_components/bootstrap/dist/css/bootstrap.css",
            "bower_components/angular/angular-csp.css",
            // "bower_components/bootstrap/dist/css/bootstrap-theme.css"
        ];

        this.fontFiles = [
            "bower_components/bootstrap/dist/fonts/**"
        ];
    }
    return GulpConfig;
})();
module.exports = GulpConfig;
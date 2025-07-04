/// <reference path="C:\Projects\eRx\Angular\ePrescribe\eRxWeb\SPA/Lib/rxjs/bundles/Rx.min.js" />
/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp");
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var del = require('del');

var shell = require('gulp-shell');
var clean = require('gulp-clean');
var htmlreplace = require('gulp-html-replace');
var runSequence = require('run-sequence');
var Builder = require('systemjs-builder');
var builder = new Builder('', './SPA/systemjs.config.js');

var bundleHash = '2017.1';
var mainBundleName = bundleHash + '.main.bundle.js';
var vendorBundleName = bundleHash + '.vendor.bundle.js';
var paths = {
    webroot: "./wwwroot/"
};

gulp.task('clean', function () {
    //del is an async function and not a gulp plugin (just standard nodejs)
    //It returns a promise, so make sure you return that from this task function
    //  so gulp knows when the delete is complete
 //   return del(['./SPA/lib/**/*']);
});
gulp.task("copy:node",
    function() {
        //gulp.src(["node_modules/angular2/bundles/**/*"]).pipe(gulp.dest(paths.webroot + '/lib/angular2/'))
        //gulp.src("node_modules/typescript/lib/**/*").pipe(gulp.dest(paths.webroot + '/lib/typescript/'))
        //gulp.src("node_modules/rxjs/bundles/**/*").pipe(gulp.dest(paths.webroot + '/lib/Rx/'))
        //gulp.src("node_modules/systemjs/dist/**/*").pipe(gulp.dest(paths.webroot + '/lib/System/'))
        //gulp.task('default', function () {
        //    gulp.src(paths.scripts).pipe(gulp.dest(paths.webroot + '/scripts'))
        //});

        gulp.src([
                'reflect-metadata/Reflect.js',
                'rxjs/**',
                'zone.js/dist/**',
                '@angular/**',
                'jquery/dist/jquery.*js',
                'bootstrap/dist/js/bootstrap.*js',
                'jasmine-core/**',
                'symbol-observable/**',
                'core-js/**',
                'systemjs/**',
            ],
            {
                cwd: "node_modules/**"
            })
            .pipe(gulp.dest("./SPA/lib"));

    });
// This is main task for production use
gulp.task('dist', function (done) {
    runSequence('bundle', function () {//'copy_assets'
        done();
    });
});

gulp.task('bundle', ['bundle:vendor', 'bundle:app'], function () {
    return gulp.src('spa.aspx')
        .pipe(htmlreplace({
            'app': mainBundleName,
            'vendor': vendorBundleName
        }));
        
});

gulp.task('replace',  function () {
    return gulp.src('Spa.aspx')
        .pipe(htmlreplace({
            'js': ['./SPA/dist/' + vendorBundleName,'./SPA/dist/' + mainBundleName ]
        })).pipe(gulp.dest('.'));

});

gulp.task('bundle:vendor', function () {
   
    return builder
        .buildStatic('./SPA/vendor.js', './SPA/dist/' + vendorBundleName,
            {
                cwd: "node_modules/**", minify: true
            })
        .catch(function (err) {
            console.log('Vendor bundle error');
            console.log(err);
        });
});

gulp.task('bundle:app', function () {
    return builder
        .buildStatic('./SPA/boot.js', './SPA/dist/' + mainBundleName, { minify: true })
        .catch(function (err) {
            console.log('App bundle error');
            console.log(err);
        });
});

gulp.task('compile_ts', ['clean:ts'], shell.task([
    'tsc'
]));



gulp.task('clean', ['clean:ts', 'clean:dist']);

gulp.task('clean:dist', function () {
    return gulp.src(['./SPA/dist'], { read: false })
        .pipe(clean());
});

gulp.task('clean:ts', function () {
    return gulp.src(['./app/**/*.js', './app/**/*.js.map'], { read: false })
        .pipe(clean());
});

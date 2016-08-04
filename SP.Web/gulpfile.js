var useref = require('gulp-useref');
var uglify = require('gulp-uglify');
var gulpIf = require('gulp-if');
var cssnano = require('gulp-cssnano');
var imagemin = require('gulp-imagemin');
var cache = require('gulp-cache');
var del = require('del');
var runSequence = require('run-sequence');
var rev = require('gulp-rev');
var revReplace = require('gulp-rev-replace');
var htmlmin = require('gulp-htmlmin');
var uncss = require('gulp-uncss');

gulp.task('html', ['styles'], () => {
    var mainFile = 'index.html';
    return gulp.src(mainFile)
        .pipe(useref())
        .pipe(gulpIf('*.js', uglify()))
        .pipe(gulpIf('*.css', cssnano({ safe: true, autoprefixer: false })))
        .pipe(gulpIf('*.css', uncss({ html: mainFile })))
        .pipe(gulpIf('*.js', rev()))
        .pipe(gulpIf('*.css', rev()))
        .pipe(revReplace())
        .pipe(gulpIf('*.html', htmlmin({ collapseWhitespace: true })))
        .pipe(gulp.dest('dist'));
});

// Optimizing Images 
gulp.task('images', function () {
    return gulp.src('app/images/**/*.+(png|jpg|jpeg|gif|svg)')
      // Caching images that ran through imagemin
      .pipe(cache(imagemin({
          interlaced: true,
      })))
      .pipe(gulp.dest('dist/images'))
});

// Copying fonts 
gulp.task('fonts', function () {
    return gulp.src('app/fonts/**/*')
      .pipe(gulp.dest('dist/fonts'))
})

// Cleaning 
gulp.task('clean', function () {
    return del.sync('dist').then(function (cb) {
        return cache.clearAll(cb);
    });
})

gulp.task('clean:dist', function () {
    return del.sync(['dist/**/*', '!dist/images', '!dist/images/**/*']);
});

gulp.task('build', function (callback) {
    runSequence(
      'clean:dist',
      ['html', 'images', 'fonts'],
      callback
  )
});
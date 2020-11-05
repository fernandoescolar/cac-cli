const fs = require('fs');
const glob = require('glob');

glob('bin/*', function (er, files) {
  files.forEach(file => { if (!file.endsWith('js')) fs.unlinkSync(file); });
});
glob('**/*.zip', function (er, files) {
  files.forEach(file => fs.unlinkSync(file));
});

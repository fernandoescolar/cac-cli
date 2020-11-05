const unzipper = require('unzipper');
const request = require('superagent');
const fs = require('fs');
const version = require('./package.json').version;
const isx64 = process.arch === 'x64' || process.env.hasOwnProperty('PROCESSOR_ARCHITEW6432');
const os = process.platform === 'win32' && isx64 ? 'win-x64'
       : process.platform === 'win32' ? 'win-x86'
       : process.platform === 'darwin' ? 'osx-x64'
       : 'linux-x64';
const baseUrl = `https://github.com/fernandoescolar/cac-cli/releases/download/v${version}`;
const zipFile = `cac-cli_${os}.zip`;
const url = `${baseUrl}/${zipFile}`;

console.log(`dowloading cac-cli ${version} for ${os}...`);
request
  .get(url)
  .on('error', function(error) {
    console.log(error);
  })
  .pipe(fs.createWriteStream(zipFile))
  .on('finish', function() {
    console.log('finished dowloading.');
    fs.createReadStream(zipFile)
      .pipe(unzipper.Extract({ path: 'bin' }))
      .on('end', () => {
        var binary = process.platform === 'win32' ? 'bin/cac.exe' : 'bin/cac';
        fs.chmodSync(binary, 0o755);

        console.log('finished installation.');

        fs.unlinkSync(zipFile);
        console.log('finished cleanup.');
      });
  });
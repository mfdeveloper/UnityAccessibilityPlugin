#!/usr/bin/env node

//#region Node libraries

import path from "node:path";
import fs from "node:fs";
import fsPromises from "node:fs/promises";
import { cwd } from 'node:process';
import { parseArgs } from "node:util";

//#endregion

//#region Community libraries

import { mkdirp } from "mkdirp";
import { glob } from "glob";
import { readPackage, readPackageSync } from 'read-pkg';
import chalk from 'chalk';
import inquirer from "inquirer";
import compressing from "compressing";

import { error, info, tip, success, warning } from './lib/chalk-colors-themes.js';
import { WorkspaceGithubReleaseDeps } from "./download-tarball-dependencies.js";

//#endregion

// TODO: Move this class to a npm package
export class WorkspaceCompressedDeps {
    
    args = {
        packagesDir: 'Packages',
        skipDownload: false,
        verbose: false
    };

    tarballInternalDir = 'package';
    tarballExtension = 'tgz';

    //#region Private Fields

    #githubReleaseDeps;
    #filesToExtract = [];
    #argumentsDefinitions = {
        packagesDir: {
            default: this.args.packagesDir,
            type: 'string',
            short: 'p'
        },
        skipDownload: {
            default: false,
            type: 'boolean',
            short: 's'
        },
        verbose: {
            default: this.args.verbose,
            type: 'boolean',
            short: 'v'
        },
        repository: {
            default: '',
            type: 'string',
            short: 'r'
        },
        downloadPath: {
            default: '',
            type: 'string',
            short: 'd'
        }
    };

    //#endregion

    get argsDefinitions() {
        return this.#argumentsDefinitions;
    }

    constructor() {

        this.initArguments();

        this.#githubReleaseDeps = new WorkspaceGithubReleaseDeps({
            parseArguments: false,
            cliArguments: this.args
        });

        this.#githubReleaseDeps.args.downloadPath = this.args.packagesDir;
        this.#githubReleaseDeps.tarballExtension = this.tarballExtension;
    }

    initArguments() {

        this.#parseDefaultPackagesDir();

        // ParseArguments: CLI parameters definitions
        const { values } = parseArgs({ options: this.#argumentsDefinitions });
        this.args = values;

        return this.args;
    }

    async run() {

        console.log(tip(`Lookup for tarball WORKSPACE dependencies .${this.tarballExtension} files started...`));
        
        if (!this.args.skipDownload) {
            
            await this.#githubReleaseDeps.downloadAssetsDeps();
        }

        let packages = Array.of(new fs.Dirent());

        try {
            packages = await fsPromises.readdir(this.args.packagesDir, { withFileTypes: true });
        } catch (err) {
            console.log(error(`Unable to scan the workspace directory: '${this.args.packagesDir}'`, err));
        }
        
        for await (const packageFileOrDir of packages) {

            if (!packageFileOrDir.name || packageFileOrDir.name.length == 0) {
                continue;
            }
    
            let packagePath = '';
            let tarballFile = '';
            
            if (packageFileOrDir.isDirectory()) {
                
                packagePath = `${this.args.packagesDir}/${packageFileOrDir.name}`;
    
                try {
                    
                        // Try to find the VERSION for tarball .tgz file
                    tarballFile = await this.tarballPathFromVersion(packagePath, packageFileOrDir.name);
    
                } catch (err) {
    
                    if (err) {
                        console.warn(warning(`Cannot find a package.json in: '${packagePath}'`));
                    }
    
                    tarballFile = await this.#tarballPathPrompt(packagePath);
                }
                
                try {
                    
                    await this.extract(tarballFile, packagePath);
                } catch (errTarball) {
                        // BYPASS tarball file if the extract() emits an error: 
                        // (e.g file doesn't exists or corrupted, destination package path is full...) !!
                }
            
            } else if (packageFileOrDir.isFile() && path.extname(packageFileOrDir.name) === `.${this.tarballExtension}`) {
    
                tarballFile = `${this.args.packagesDir}/${packageFileOrDir.name}`;
    
                try {
                    packagePath = await this.createPackageDir(this.args.packagesDir, tarballFile);
    
                    try {
                        
                        await this.extract(tarballFile, packagePath);
                    } catch (errTarball) {
                        // BYPASS tarball file if the extract() emits an error: 
                        // (e.g file doesn't exists or corrupted, destination package path is full...) !!
                    }
                    
                } catch (err) {
                    if (err) {
                        console.log(error(err));
                    }
                }   
            }
        }
    }

    async extract(tarballFile = '', destinationDir = '', options = { verbose: this.args.verbose }) {
        
        try {
            
            await fsPromises.access(tarballFile, fsPromises.constants.F_OK | fsPromises.constants.R_OK);
        } catch (accessErr) {
            console.info(info(`${tarballFile} doesn't exists. Bypassing...`));
            
            return Promise.reject(skipExtractMessage);
        }
        
        const destinationContent = await fsPromises.readdir(destinationDir);
        let skipExtractMessage = '';

        if (destinationContent.length > 2) {
            skipExtractMessage = `Destination directory: ${destinationDir} isn't empty. Skiping the file: ${tarballFile}`;
            console.warn(warning(skipExtractMessage));

            return Promise.reject(skipExtractMessage);
        }

        const packageName = this.packageNameFromTarballFile(tarballFile);
        if (!this.#filesToExtract.find((value) => packageName == value)) {
            this.#filesToExtract.push(packageName);
        } else {
            skipExtractMessage = `The file: '${tarballFile}' is already marked to be extracted`;
            console.warn(warning(skipExtractMessage));

            return Promise.reject(skipExtractMessage);
        }

        console.warn(warning(`Extracting file: ${tarballFile}...`));
        
        return new compressing.tgz.UncompressStream({ source: tarballFile, strip: 1 })
            .on('entry', async (header, stream, next) => {

                /* 
                * header.type => file | directory
                * header.name => path name
                */

                stream.on('end', next);

                const normalizedFile = header.name.replace(this.tarballInternalDir, '');
                
                if (header.type === 'file') {
                    
                    // file

                    const parentDir = path.dirname(normalizedFile);

                    if (parentDir != path.sep) {
                    
                        try {

                            const extractDir = path.join(destinationDir, parentDir);
                            await mkdirp(extractDir);
                        } catch (err) {
                            if (err) {
                                console.log(error(err));
                            }
                            stream.resume();
                        }
                    }

                    const extractFile = path.join(destinationDir, normalizedFile);
                    stream.pipe(
                        fs.createWriteStream(extractFile)
                    ).on('finish', () => {
                        if (options.verbose) {
                            console.log(extractFile);
                        }
                    });
                    
                } else {

                    // directory
                    
                    try {

                        await mkdirp(path.join(destinationDir, normalizedFile));

                        if (options.verbose) {
                            console.log(`Directory: ${extractDir}`);
                        }
                    } catch (err) {
                        if (err) {
                            console.log(error(err));
                        }
                        stream.resume();
                    }
                }
            })
            .on('finish', () => {
                // Uncompressing/Extraction is DONE!
                console.log(success(`Extraction of package: '${chalk.bold.green.underline(tarballFile)}' DONE!`));

                const extractedFileIndex = this.#filesToExtract.indexOf(packageName);
                if (extractedFileIndex != -1) {
                    this.#filesToExtract.splice(extractedFileIndex, 1);
                }
            })
            .on('error', (reason) => {
                console.log(error(reason));
            });
    }

    async createPackageDir(rootPath, tarballFileName) {

        const dirName = this.packageNameFromTarballFile(tarballFileName);
        const packagePath = `${rootPath}/${dirName}`;

        await mkdirp(packagePath);

        return packagePath;
    }

    // TODO: Move this method to another "Common" class, to reuse among .js scripts
    packageNameFromTarballFile(fileName = '') {

        const versionNumbersRegex = /-\d*.*/;
        const dirName = path.basename(fileName, this.tarballExtension).replace(versionNumbersRegex, '');
        
        return dirName;
    }

    // TODO: Try download a .tgz, if the "packagePath" only have a package.json file
    async tarballPathFromVersion(packagePath = '', fileName = '') {

        const packageJson = await readPackage({ cwd: packagePath });
        const version = packageJson.version || '';
        let tarballFile = '';

        if (version.length > 0) {
            tarballFile = `${packagePath}/${fileName}-${packageJson.version}.${this.tarballExtension}`;
        }

        return tarballFile;
    }

    //#region Private Methods

    async #tarballPathPrompt(packagePath = this.args.packagesDir) {
                
        // Show the all existent tarball .tgz files, inside of "packagePath"
        const globResults = await glob(`${packagePath}/**/*.${this.tarballExtension}`);
        let tarballFile = '';
        
        if (globResults.length > 1) {
            
            const promptMessage = `There are many tarball files in: ${chalk.bold.green.underline(packagePath)}. Please, select only one`;

            await inquirer.prompt([
                {
                    type: 'list',
                    name: 'tarballFile',
                    message: promptMessage,
                    choices: globResults,
                    default: globResults[0]
                }
            ]).then((answers) => {

                tarballFile = answers.tarballFile;
            });

        } else if (globResults.length == 1) {

            tarballFile = globResults[0];
        }

        return tarballFile;
    }

    #parseDefaultPackagesDir() {

        let rootPackageJson = {};

        try {
            rootPackageJson = readPackageSync();
        } catch (_) {
            const packageJsonError = `Root package.json file not found in: '${cwd()}'. Using '${this.args.packagesDir}' as default workspace directory`;
            console.log(info(packageJsonError));
        }

        if (rootPackageJson.workspaces?.length > 0) {
            let defaultWorkspacesDir = rootPackageJson.workspaces[0] || '';

            if (defaultWorkspacesDir && defaultWorkspacesDir.length > 0) {
                defaultWorkspacesDir = defaultWorkspacesDir.replace(/\/\*.*/, '');
                
                if (this.args.packagesDir != defaultWorkspacesDir) {
                    this.args.packagesDir = defaultWorkspacesDir;
                }
            }
        }

        return rootPackageJson;
    }

    //#endregion
}

export const compressedDeps = new WorkspaceCompressedDeps();
compressedDeps.run();

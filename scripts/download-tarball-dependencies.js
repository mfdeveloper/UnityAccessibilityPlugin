#!/usr/bin/env node

import path from 'node:path';
import fs from "node:fs";
import { parseArgs } from "node:util";
import { mkdirp } from "mkdirp";

import chalk from 'chalk';
import releaseDownloader from "@fohlen/github-release-downloader";
import gitRemoteOriginUrl from 'git-remote-origin-url';

import { info, tip, success, warning } from './lib/chalk-colors-themes.js';

export class WorkspaceGithubReleaseDeps {

    args = {
        repository: '',
        downloadPath: 'Packages'
    };

    tarballExtension = 'tgz';

    //#region Private Fields

    #argumentsDefinitions = {
        repository: {
            default: this.args.repository,
            type: 'string',
            short: 'r'
        },
        downloadPath: {
            default: this.args.downloadPath,
            type: 'string',
            short: 'd'
        }
    }

    //#endregion

    get argsDefinitions() {
        return this.#argumentsDefinitions;
    }

    constructor(options = { parseArguments: true, cliArguments: {} }) {
        
        this.initArguments(options);
    }

    initArguments(options = { parseArguments: true, cliArguments: {} }) {

        // ParseArguments: CLI parameters definitions
        
        if (options.parseArguments) {
            const { values } = parseArgs({ options: this.#argumentsDefinitions});
            this.args = values;
        } else if (options.cliArguments && Object.entries(options.cliArguments).length > 0) {
            // Merge external arguments from: "options.cliArguments" with: "this.args"
            for (const name in options.cliArguments) {
                if (Object.hasOwnProperty.call(this.args, name)) {
                    this.args[name] = options.cliArguments[name];
                }
            }
        }

        return this.args;
    }

    async run() {
        await this.downloadAssetsDeps();
    }

    async downloadAssetsDeps() {

        await this.#parseGitRepo();

        console.log(tip(`Fetch remote ${this.tarballExtension} files to download started...`));

        const release = await releaseDownloader.getReleaseByVersion(this.args.repository);
        const downloadedDeps = [];

        if (release && release.assets) {
            
            const tarballAssets = release.assets.filter(asset => this.filterValidAsset(asset));

            if (tarballAssets.length > 0) {
                
                for await (const asset of tarballAssets) {
                    
                    const packageName = this.packageNameFromTarballFile(asset.name);
                    const destinationDir = path.join(this.args.downloadPath, packageName);

                    if (!fs.existsSync(destinationDir)) {
                        await mkdirp(destinationDir);
                    }
                    
                    try {
                        const downloaded = await releaseDownloader.downloadAsset(asset.browser_download_url, asset.name, destinationDir);
                        console.log(success(`Downloaded tarball archive at: '${chalk.bold.green.underline(downloaded)}!'`));
    
                        downloadedDeps.push(downloaded);
                    } catch (downloadErr) {
                        console.warn(warning(`Error to download remote asset: ${asset.browser_download_url}. Are you offline?`), downloadErr);

                        return Promise.reject(downloadErr);
                    }
                }
            } else {
                console.log(info(`There aren't any tarball dependencies files on remote release: '${release.html_url}'. DOWNLOAD phase skipped!!`));
            }
        }

        return downloadedDeps;
    }

    gitRemoteSlug(remoteUrl = '') {
        const repoRegex = /(git@*.*:|(?:https?|git):\/\/?.*\.\w*\/)/;
        let slug = remoteUrl.replace(repoRegex, '');

        if (slug.length > 0) {
            slug = slug.replace(/\.git$/, '');
        }

        return slug;
    }

    // TODO: Move this method to another "Common" class, to reuse among .js scripts
    packageNameFromTarballFile(fileName = '') {

        const versionNumbersRegex = /-\d*.*/;
        const dirName = path.basename(fileName, this.tarballExtension).replace(versionNumbersRegex, '');
        
        return dirName;
    }

    filterValidAsset(asset = {}) {
        
        const assetFile = {
            extension: path.extname(asset.browser_download_url),
            packageName: this.packageNameFromTarballFile(asset.name)
        };

        const tarballFile = path.join(this.args.downloadPath, assetFile.packageName, asset.name);
        const tarballExistsLocally = fs.existsSync(tarballFile);

        if (tarballExistsLocally) {
            console.log(info(`A tarball file: '${tarballFile}' for remote release asset: ${asset.browser_download_url} already exists locally. DOWNLOAD isn't necessary...`));
            
            return false;
        }
        
        return assetFile.extension == `.${this.tarballExtension}` || path.extname(asset.name) == `.${this.tarballExtension}`;;
    }

    //#region Private Methods
    
    async #parseGitRepo() {
        
        if (this.args.repository.length == 0) {
            const remoteUrl = await gitRemoteOriginUrl();
            this.args.repository = this.gitRemoteSlug(remoteUrl);
        }

        return this.args.repository;
    }

    //#endregion
}

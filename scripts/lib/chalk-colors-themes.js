import chalk from 'chalk';

// Chalk: console message colors themes
const chalkThemes = {
    error: chalk.bold.red,
    warning: chalk.yellow,
    info: chalk.blue,
    tip: chalk.bold.cyan,
    success: chalk.bold.green
};

export const { error, warning, info, tip, success } = chalkThemes;
export default chalkThemes;

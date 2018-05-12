import * as fs from 'fs';

export async function readFileAsString(filePath: string): Promise<string> {
    return new Promise<string>((resolve, reject) => {
        fs.readFile(filePath, (err, data) => {
            if (!err) {
                resolve(data.toString());;
            } else {
                reject(err);
            }
        });
    });
}

try {
    'export const isDebug = false;' | Out-File ./src/enviroment.ts
    vsce package
} 
finally {
    'export const isDebug = true;' | Out-File ./src/enviroment.ts
}
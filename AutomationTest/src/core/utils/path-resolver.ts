import path from 'path';
import fs from 'fs';

export const findProjectRoot = (): string => {
  let dir = __dirname;
  while (!fs.existsSync(path.join(dir, 'package.json'))) {
    const parent = path.dirname(dir);
    if (parent === dir) throw new Error('Project root not found');
    dir = parent;
  }
  return dir;
};

export const resolvePaths = (relativePaths: string[]): string[] => {
  const projectRoot = findProjectRoot();
  return relativePaths.map(p => {
    const fullPath = path.resolve(projectRoot, 'src', p);
    if (!fs.existsSync(fullPath)) {
      throw new Error(`File not found: ${fullPath}`);
    }
    return fullPath;
  });
};

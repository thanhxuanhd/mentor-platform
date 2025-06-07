import path from 'path';
import fs from 'fs';
import test from '@playwright/test';
import { ResourcePage } from '../../pages/resources/resource-management-page';

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

export async function createAndVerifyResource(data: any, resourcePage: ResourcePage) {
  await resourcePage.goToCoursePage();
  await resourcePage.selectResourceModal();
  await resourcePage.clickAddResourceButton();
  await resourcePage.inputTitle(data.title);
  await resourcePage.inputDescription(data.description);
  const resolvedPaths = resolvePaths(data.fileName);
  await resourcePage.uploadResource(resolvedPaths);
  await resourcePage.clickCreateButton();
}

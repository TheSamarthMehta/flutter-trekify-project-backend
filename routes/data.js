const express = require('express');
const XLSX = require('xlsx');
const path = require('path');
const fs = require('fs');

const router = express.Router();

// Helpers for robust header-based extraction
function normalizeHeader(header) {
  return String(header || '')
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, ' ')
    .trim()
    .replace(/\s+/g, ' ');
}

function buildHeaderIndex(headers) {
  const indexByKey = {};
  headers.forEach((h, idx) => {
    const key = normalizeHeader(h);
    if (key) indexByKey[key] = idx;
  });
  return indexByKey;
}

function findFirstIndex(indexByKey, candidates) {
  for (const c of candidates) {
    const key = normalizeHeader(c);
    if (indexByKey[key] !== undefined) return indexByKey[key];
  }
  return undefined;
}

function parseBoolean(value) {
  if (value === undefined || value === null) return false;
  const v = String(value).toLowerCase().trim();
  return v === 'yes' || v === 'true' || v === 'y' || v === '1';
}

// Normalize "Guide needed?" into exact strings: YES / NO / RECOMMENDED (keep OPTIONAL / NOT NEEDED as-is)
function parseGuideNeededString(value) {
  if (value === undefined || value === null) return '';
  const v = String(value).toLowerCase().trim();
  if (['required', 'yes', 'y', 'true', '1'].includes(v)) return 'YES';
  if (['not needed', 'not required', 'no', 'false', '0', 'n'].includes(v)) return 'NO';
  if (['recommended', 'recommend', 'advisable', 'advised'].includes(v)) return 'RECOMMENDED';
  if (['optional', 'maybe'].includes(v)) return 'OPTIONAL';
  return v.toUpperCase();
}

function parseYesNoString(value) {
  if (value === undefined || value === null) return 'NO';
  return parseBoolean(value) ? 'YES' : 'NO';
}

function detectImageUrl(row) {
  for (let j = 0; j < row.length; j++) {
    const cellValue = row[j];
    if (
      cellValue && typeof cellValue === 'string' &&
      (cellValue.includes('cloudinary.com') ||
        cellValue.includes('res.cloudinary') ||
        cellValue.startsWith('https://res.cloudinary'))
    ) {
      return cellValue;
    }
  }
  return '';
}

function parseTreksFromWorksheet(worksheet) {
  const jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
  if (jsonData.length === 0) {
    return { treks: [], headers: [] };
  }

  const headers = jsonData[0];
  const indexByKey = buildHeaderIndex(headers);

  const idx = {
    serialNumber: findFirstIndex(indexByKey, ['serial no', 'serial number', 's no', 'sno', 'sr no']),
    trekName: findFirstIndex(indexByKey, ['trek name', 'name']),
    state: findFirstIndex(indexByKey, ['state']),
    trekType: findFirstIndex(indexByKey, ['trek type', 'type']),
    difficultyLevel: findFirstIndex(indexByKey, ['difficulty', 'difficulty level']),
    season: findFirstIndex(indexByKey, ['season', 'best season', 'best time']),
    duration: findFirstIndex(indexByKey, ['duration']),
    distance: findFirstIndex(indexByKey, ['distance']),
    maxAltitude: findFirstIndex(indexByKey, ['max altitude', 'altitude', 'height', 'maxaltitude']),
    trekDescription: findFirstIndex(indexByKey, ['description', 'trek description', 'about']),
    image: findFirstIndex(indexByKey, ['image', 'image url', 'image link', 'cloudinary url', 'photo']),
    ageGroup: findFirstIndex(indexByKey, ['age group', 'age', 'recommended age']),
    guideNeeded: findFirstIndex(indexByKey, ['guide needed', 'guide need', 'need guide', 'guide required']),
    snowTrek: findFirstIndex(indexByKey, ['snow trek', 'snow', 'is snow trek']),
    recommendedGear: findFirstIndex(indexByKey, ['recommended gear', 'gear', 'gears', 'what to carry'])
  };

  const treks = [];
  for (let i = 1; i < jsonData.length; i++) {
    const row = jsonData[i];
    if (!row || row.length === 0) continue;

    const trekName = idx.trekName !== undefined ? row[idx.trekName] : row[1];
    const state = idx.state !== undefined ? row[idx.state] : row[2];
    if (!trekName || !state) continue;

    const imageFromHeader = idx.image !== undefined ? row[idx.image] : undefined;
    const image = imageFromHeader && typeof imageFromHeader === 'string' && imageFromHeader.startsWith('http')
      ? imageFromHeader
      : detectImageUrl(row);

    const guideString = idx.guideNeeded !== undefined ? parseGuideNeededString(row[idx.guideNeeded]) : '';
    const snowString = idx.snowTrek !== undefined ? parseYesNoString(row[idx.snowTrek]) : 'NO';

    const trek = {
      id: i - 1,
      serialNumber: idx.serialNumber !== undefined ? row[idx.serialNumber] : (row[0] || i),
      trekName: trekName || '',
      state: state || '',
      trekType: idx.trekType !== undefined ? (row[idx.trekType] || '') : (row[3] || ''),
      difficultyLevel: idx.difficultyLevel !== undefined ? (row[idx.difficultyLevel] || '') : (row[4] || ''),
      season: idx.season !== undefined ? (row[idx.season] || '') : (row[5] || ''),
      duration: idx.duration !== undefined ? (row[idx.duration] || '') : (row[6] || ''),
      distance: idx.distance !== undefined ? (row[idx.distance] || '') : (row[7] || ''),
      maxAltitude: idx.maxAltitude !== undefined ? (row[idx.maxAltitude] || '') : (row[8] || ''),
      trekDescription: idx.trekDescription !== undefined ? (row[idx.trekDescription] || '') : (row[9] || ''),
      ageGroup: idx.ageGroup !== undefined ? (row[idx.ageGroup] || '') : '',
      guideNeeded: guideString,
      snowTrek: snowString,
      recommendedGear: idx.recommendedGear !== undefined ? (row[idx.recommendedGear] || '') : '',
      image
    };

    treks.push(trek);
  }

  return { treks, headers };
}

// @route   GET /api/data/load-excel
// @desc    Load trek data from Excel file
// @access  Public
router.get('/load-excel', async (req, res) => {
  try {
    const excelPath = path.join(__dirname, '..', 'data', 'Flutter Data Set.xlsx');
    if (!fs.existsSync(excelPath)) {
      return res.status(404).json({
        success: false,
        message: 'Excel file not found',
        path: excelPath
      });
    }

    const workbook = XLSX.readFile(excelPath);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];

    const { treks, headers } = parseTreksFromWorksheet(worksheet);

    res.json({
      success: true,
      message: `Successfully loaded ${treks.length} treks from Excel`,
      count: treks.length,
      headers,
      data: treks
    });
  } catch (error) {
    console.error('Error loading Excel data:', error);
    res.status(500).json({
      success: false,
      message: 'Error loading trek data from Excel',
      error: error.message
    });
  }
});

// @route   GET /api/data/
// @desc    Get all trek data
// @access  Public
router.get('/', async (req, res) => {
  try {
    const excelPath = path.join(__dirname, '..', 'data', 'Flutter Data Set.xlsx');
    if (!fs.existsSync(excelPath)) {
      return res.status(404).json({
        success: false,
        message: 'Excel file not found'
      });
    }

    const workbook = XLSX.readFile(excelPath);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const { treks } = parseTreksFromWorksheet(worksheet);

    res.json({
      success: true,
      message: `Successfully loaded ${treks.length} treks from Excel`,
      count: treks.length,
      data: treks
    });
  } catch (error) {
    console.error('Error fetching trek data:', error);
    res.status(500).json({
      success: false,
      message: 'Error fetching trek data'
    });
  }
});

// @route   GET /api/data/states
// @desc    Get unique states
// @access  Public
router.get('/states', async (req, res) => {
  try {
    // Load trek data and extract unique states
    const excelPath = path.join(__dirname, '..', 'data', 'Flutter Data Set.xlsx');
    const workbook = XLSX.readFile(excelPath);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
    
    const states = new Set();
    for (let i = 1; i < jsonData.length; i++) {
      const state = jsonData[i][2]; // State is in column 3 (index 2)
      if (state && typeof state === 'string') {
        states.add(state.trim());
      }
    }

    res.json({
      success: true,
      data: Array.from(states).sort()
    });

  } catch (error) {
    console.error('Error fetching states:', error);
    res.status(500).json({
      success: false,
      message: 'Error fetching states',
      error: error.message
    });
  }
});

// @route   GET /api/data/trek-types
// @desc    Get unique trek types
// @access  Public
router.get('/trek-types', async (req, res) => {
  try {
    const excelPath = path.join(__dirname, '..', 'data', 'Flutter Data Set.xlsx');
    const workbook = XLSX.readFile(excelPath);
    const worksheet = workbook.Sheets[workbook.SheetNames[0]];
    const jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
    
    const trekTypes = new Set();
    for (let i = 1; i < jsonData.length; i++) {
      const trekType = jsonData[i][3]; // Trek type is in column 4 (index 3)
      if (trekType && typeof trekType === 'string') {
        trekTypes.add(trekType.trim());
      }
    }

    res.json({
      success: true,
      data: Array.from(trekTypes).sort()
    });

  } catch (error) {
    console.error('Error fetching trek types:', error);
    res.status(500).json({
      success: false,
      message: 'Error fetching trek types',
      error: error.message
    });
  }
});

module.exports = router;

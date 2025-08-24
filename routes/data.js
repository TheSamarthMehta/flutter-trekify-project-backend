const express = require('express');
const XLSX = require('xlsx');
const path = require('path');
const fs = require('fs');

const router = express.Router();

// @route   GET /api/data/load-excel
// @desc    Load trek data from Excel file
// @access  Public
router.get('/load-excel', async (req, res) => {
  try {
    console.log('Loading trek data from Excel file...');
    
    // Path to Excel file
    const excelPath = path.join(__dirname, '..', 'data', 'Flutter Data Set.xlsx');
    
    if (!fs.existsSync(excelPath)) {
      return res.status(404).json({
        success: false,
        message: 'Excel file not found',
        path: excelPath
      });
    }

    // Read Excel file
    const workbook = XLSX.readFile(excelPath);
    const sheetName = workbook.SheetNames[0];
    const worksheet = workbook.Sheets[sheetName];
    
    // Convert to JSON
    const jsonData = XLSX.utils.sheet_to_json(worksheet, { header: 1 });
    
    if (jsonData.length === 0) {
      return res.status(400).json({
        success: false,
        message: 'Excel file is empty'
      });
    }

    // Process the data
    const headers = jsonData[0];
    const treks = [];
    
    console.log('Excel headers:', headers);
    console.log(`Found ${jsonData.length - 1} trek records`);

    for (let i = 1; i < jsonData.length; i++) {
      const row = jsonData[i];
      if (!row || row.length === 0) continue;

      // Based on our analysis: Col0=SerialNo, Col1=TrekName, Col2=State, etc.
      const trek = {
        id: i - 1,
        serialNumber: row[0] || i,
        trekName: row[1] || '', // Column 2 has trek names  
        state: row[2] || '',    // Column 3 has state names
        trekType: row[3] || '',
        difficultyLevel: row[4] || '',
        season: row[5] || '',
        duration: row[6] || '',
        distance: row[7] || '',
        maxAltitude: row[8] || '',
        trekDescription: row[9] || '',
        image: '' // Will be populated with exact URLs from Excel
      };

      // Look for Cloudinary URLs specifically in column 12 (index 12) based on .NET findings
      const cloudinaryColumn = row[12]; // Column 13 (index 12)
      if (cloudinaryColumn && typeof cloudinaryColumn === 'string' && 
          (cloudinaryColumn.includes('cloudinary.com') || 
           cloudinaryColumn.includes('res.cloudinary') ||
           cloudinaryColumn.startsWith('https://res.cloudinary'))) {
        trek.image = cloudinaryColumn;
        console.log(`Found Cloudinary URL for ${trek.trekName}: ${cloudinaryColumn}`);
      } else {
        // Fallback: Look in other columns if not found in column 13
        for (let j = 10; j < row.length; j++) {
          const cellValue = row[j];
          if (cellValue && typeof cellValue === 'string' && 
              (cellValue.includes('cloudinary.com') || 
               cellValue.includes('res.cloudinary') ||
               cellValue.startsWith('https://res.cloudinary'))) {
            trek.image = cellValue;
            console.log(`Found Cloudinary URL in column ${j+1} for ${trek.trekName}: ${cellValue}`);
            break;
          }
        }
      }

      // Only add trek if it has valid data
      if (trek.trekName && trek.state) {
        treks.push(trek);
      }
    }

    console.log(`Processed ${treks.length} valid treks`);
    console.log('Sample trek with image:', treks.find(t => t.image) || treks[0]);

    res.json({
      success: true,
      message: `Successfully loaded ${treks.length} treks from Excel`,
      count: treks.length,
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
    // For now, redirect to load-excel endpoint
    // In a real app, you'd cache this data in MongoDB
    const response = await fetch(`${req.protocol}://${req.get('host')}/api/data/load-excel`);
    const data = await response.json();
    
    res.json(data);
  } catch (error) {
    console.error('Error fetching trek data:', error);
    res.status(500).json({
      success: false,
      message: 'Error fetching trek data',
      error: error.message
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

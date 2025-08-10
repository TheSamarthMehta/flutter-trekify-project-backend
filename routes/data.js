// trekify-backend/routes/data.js
const express = require('express');
const router = express.Router();
const xlsx = require('xlsx');
const path = require('path');

// --- Load Excel File Data ---
// This assumes your Excel file is in a 'data' subfolder of your backend project
const workbook = xlsx.readFile(path.join(__dirname, '..', 'data', 'Flutter Data Set.xlsx'));
const sheetName = workbook.SheetNames[0];
const sheet = workbook.Sheets[sheetName];
let data = xlsx.utils.sheet_to_json(sheet); // Use 'let' to allow modification (for DELETE)

// --- Define Data Routes ---

// @route   GET api/data
// @desc    Get all trek data
// @access  Public
router.get('/', (req, res) => {
  res.json(data);
});

// @route   GET api/data/state/:stateName
// @desc    Get treks by state name
// @access  Public
router.get('/state/:stateName', (req, res) => {
  const stateParam = decodeURIComponent(req.params.stateName).toLowerCase();
  if (!stateParam) {
    return res.status(400).json({ message: 'State name is required' });
  }
  const filtered = data.filter(item => {
    const state = item["State"] || "";
    return state.toLowerCase().includes(stateParam);
  });
  if (filtered.length === 0) {
    return res.status(404).json({ message: 'No data found for this state.' });
  }
  res.json(filtered);
});

// Add any other data-related routes (GET by ID, DELETE, etc.) here...
// For example:
// @route   GET api/data/id/:id
// @desc    Get trek by Sr No.
// @access  Public
router.get('/id/:id', (req, res) => {
    const idParam = req.params.id;
    const row = data.find(item => String(item["Sr No."]) === idParam);
    if (row) {
        res.json(row);
    } else {
        res.status(404).json({ message: 'Data not found with this ID.' });
    }
});


module.exports = router;
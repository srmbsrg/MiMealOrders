-- MiMealOrders: Add AI nutrition lookup columns to MenuItems
-- Migration: 2026-04-11
-- Run once against MiMealOrdering database.
-- All columns are nullable so existing rows are unaffected.

ALTER TABLE MenuItems ADD
    intCalories         INT           NULL,
    decTotalFatG        DECIMAL(5,1)  NULL,
    decSaturatedFatG    DECIMAL(5,1)  NULL,
    decCholesterolMg    DECIMAL(6,1)  NULL,
    decSodiumMg         DECIMAL(6,1)  NULL,
    decTotalCarbG       DECIMAL(5,1)  NULL,
    decDietaryFiberG    DECIMAL(5,1)  NULL,
    decSugarsG          DECIMAL(5,1)  NULL,
    decProteinG         DECIMAL(5,1)  NULL,
    bitIsAIEstimate     BIT           NULL DEFAULT(1),
    dtNutritionLookedUp DATETIME      NULL;

-- Verify
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'MenuItems'
  AND COLUMN_NAME LIKE '%alorie%'
   OR COLUMN_NAME LIKE '%Fat%'
   OR COLUMN_NAME LIKE '%Carb%'
   OR COLUMN_NAME LIKE '%Protein%'
ORDER BY ORDINAL_POSITION;

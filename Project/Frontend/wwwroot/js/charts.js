function createCo2Chart(dates, co2Data) {
	var ctx = document.getElementById('co2Chart').getContext('2d');
	new Chart(ctx, {
		type: 'line',
		data: {
			labels: dates,
			datasets: [
				{
					label: 'CO2 Emissions (kg)',
					data: co2Data,
					backgroundColor: 'rgba(75, 192, 192, 0.2)',
					borderColor: 'rgba(75, 192, 192, 1)',
					borderWidth: 1,
				},
			],
		},
		options: {
			scales: {
				y: {
					beginAtZero: true,
				},
			},
		},
	});

    alert('JS function executed!'); // For testing
	console.log('Dates:', dates);
	console.log('CO2 Data:', co2Data);
}

import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt

def generate_chart(dates, co2_data, output_path = "Project/Frontend/wwwroot/images/plot.png"):
    plt.figure(figsize=(10, 5))
    plt.plot(dates, co2_data, marker='o')
    plt.title('CO2 Emissions Over Time')
    plt.xlabel('Date')
    plt.ylabel('CO2 Emissions (kg)')
    plt.grid(True)
    plt.savefig(output_path)
    plt.close()
    return output_path
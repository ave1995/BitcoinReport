<script lang="ts">
	import type { BitcoinTableItem } from '$lib';
	import { onDestroy, onMount } from 'svelte';
	import { Chart, registerables } from 'chart.js';

	Chart.register(...registerables);

	let data: BitcoinTableItem[];

	let chart: Chart | null = null;

	function prepareChartData() {
		const labels = data.map((item) => item.time);
		const prices = data.map((item) => item.price);

		return {
			labels,
			datasets: [
				{
					label: 'Bitcoin Cena',
					data: prices,
					borderColor: 'rgba(16, 185, 129, 1)',
					backgroundColor: 'rgba(16, 185, 129, 0.2)',
					fill: true,
					tension: 0.1
				}
			]
		};
	}

	async function getBitcoins() {
		try {
			const response = await fetch('http://localhost:5186/api/bitcoin/get-bitcoins', {
				method: 'GET',
				headers: {
					'Content-Type': 'application/json'
				}
			});

			if (!response.ok) {
				throw new Error(`Error: ${response.status}`);
			}

			return await response.json();
		} catch (error) {
			console.error('Error saving Bitcoin:', error);
		}
	}

	async function updateNote(guid: string, newNote: string | null) {
		const response = await fetch(`http://localhost:5186/api/bitcoin/update-note/${guid}`, {
			method: 'PATCH',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({ note: newNote })
		});

		if (!response.ok) {
			console.error('Failed to update note:', response.statusText);
		}
	}

	async function deleteBitcoin(guid: string) {
		const response = await fetch(`http://localhost:5186/api/bitcoin/delete-bitcoin/${guid}`, {
			method: 'DELETE',
			headers: {
				'Content-Type': 'application/json'
			}
		});

		if (!response.ok) {
			console.error('Failed to delete note:', response.statusText);
		}
	}

	onMount(() => {
		getBitcoins().then((fetchedData) => {
			data = fetchedData;
			const ctx = document.getElementById('priceChart') as HTMLCanvasElement;

			chart = new Chart(ctx, {
				type: 'line',
				data: prepareChartData(),
				options: {
					responsive: true,
					plugins: {
						legend: {
							display: true,
							position: 'top'
						},
						tooltip: {
							callbacks: {
								label: function (context) {
									return `${context.dataset.label}: $${context.raw}`;
								}
							}
						}
					},
					scales: {
						x: {
							title: {
								display: true,
								text: 'Čas'
							}
						},
						y: {
							title: {
								display: true,
								text: 'Cena (EUR)'
							},
							beginAtZero: false
						}
					}
				}
			});
		});
	});

	onDestroy(() => {
		if (chart) {
			chart.destroy();
		}
	});

	let editingIndex: number | null = null;
	let newNote: string | null = null;

	function startEditing(index: number, note: string | null) {
		editingIndex = index;
		newNote = note;
	}

	async function saveNote(index: number) {
		const item = data[index];
		await updateNote(item.guid, newNote);
		item.note = newNote;
		editingIndex = null;
	}

	async function deleteRow(index: number) {
		const guid = data[index].guid;
		await deleteBitcoin(guid);

		data = data.filter((_, i) => i !== index);
	}
</script>

<button
	on:click={async () => {
		data = await getBitcoins();
		if (!chart) return;
		chart.data = prepareChartData();
		chart?.update();
	}}
	class="bg-emerald-500 text-white px-2 py-1 rounded-lg"
>
	Obnovit
</button>
<canvas id="priceChart" width="600" height="400"></canvas>
<table class="min-w-full bg-white border border-gray-200 rounded-lg shadow-md overflow-hidden">
	<thead>
		<tr class="bg-emerald-500 text-white">
			<th class="px-4 py-2 text-left">Čas</th>
			<th class="px-4 py-2 text-left">Cena</th>
			<th class="px-4 py-2 text-left">Poznámka</th>
			<th class="px-4 py-2 text-left">Akce</th>
		</tr>
	</thead>
	<tbody>
		{#each data as item, index}
			<tr class="border-b hover:bg-gray-100">
				<td class="px-4 py-2">{item.time}</td>
				<td class="px-4 py-2">{item.price.toLocaleString()}</td>
				<td class="px-4 py-2">
					{#if editingIndex === index}
						<div class="flex items-center">
							<input
								type="text"
								bind:value={newNote}
								class="border border-gray-300 rounded-md px-2 py-1 mr-2"
								placeholder="Vlož poznámku"
							/>
							<button
								on:click={async () => await saveNote(index)}
								class="hover:bg-blue-600 bg-blue-500 text-sm px-2 py-1 rounded-md border text-white"
							>
								Uložit
							</button>
						</div>
					{:else}
						<button
							on:click={() => startEditing(index, item.note)}
							on:keydown={(event) => event.key === 'Enter' && startEditing(index, item.note)}
							class="hover:underline focus:outline-none focus:ring focus:ring-blue-300"
						>
							{item.note || 'N/A'}
						</button>
					{/if}
				</td>
				<td class="px-4 py-2">
					<button
						on:click={async () => await deleteRow(index)}
						class="hover:bg-red-600 bg-red-500 text-sm text-white px-2 py-1 rounded-md border"
					>
						Smazat
					</button>
				</td>
			</tr>
		{/each}
	</tbody>
</table>

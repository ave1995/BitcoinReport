<script lang="ts">
	import type { BitcoinItem } from '$lib';

	export let bItem: BitcoinItem;

	async function saveBitcoin() {
		try {
			const response = await fetch('http://localhost:5186/api/bitcoin/save-bitcoin', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({
					time: bItem.time,
					price: bItem.price
				})
			});

			if (!response.ok) {
				throw new Error(`Error: ${response.status}`);
			}
		} catch (error) {
			console.error('Error saving Bitcoin:', error);
		}
	}
</script>

<div class="flex flex-row gap-4">
	<div class="grid grid-cols-[30%_70%] grow">
		<p>Čas:</p>
		<p>{bItem.time}</p>
		<p>CZK:</p>
		<div>{bItem.price.toLocaleString()}</div>
	</div>
	<button class="border rounded-md px-2 py-1 text-sm self-start hover:bg-blue-600 bg-blue-500 text-white" on:click={saveBitcoin}>Uložit</button>
</div>

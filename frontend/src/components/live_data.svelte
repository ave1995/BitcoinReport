<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { HubConnectionBuilder } from '@microsoft/signalr';
	import type { HubConnection } from '@microsoft/signalr/dist/esm/HubConnection';
	import { fly } from 'svelte/transition';
	import type { BitcoinItem } from '$lib';
	import LiveItem from './live_item.svelte';

	let bItems: BitcoinItem[] = [];

	let connection: HubConnection;

	async function startConnection() {
		try {
			connection = new HubConnectionBuilder()
				.withUrl('http://localhost:5186/bitcoin')
				.withAutomaticReconnect()
				.build();

			await connection.start();
			console.log('Connected to SignalR hub');

			connection.on('ReceiveBitcoinPrice', (data) => {
				bItems = [...bItems, JSON.parse(data)];
				if (bItems.length > 6) {
					bItems.shift();
				}
			});

			connection.onclose((error) => {
				console.error('Connection closed:', error);
				reconnect();
			});
		} catch (err) {
			console.error('Error connecting to SignalR hub:', err);
			setTimeout(startConnection, 30000);
		}
	}

	async function reconnect() {
		console.log('Attempting to reconnect...');
		await startConnection();
	}

	onMount(async () => await startConnection());

	onDestroy(() => {
		if (connection) {
			connection.stop();
		}
	});
</script>

<div class="flex flex-col">
	<h1 class="self-center">Live Data</h1>
	<div class="flex flex-col-reverse gap-3">
		{#each bItems as bItem, i (bItem)}
			<div
				class="px-3 py-5 border-l-emerald-500 border-4 border-t-0 border-r-0 border-b-0 rounded-sm shadow-md shadow-slate-400"
				in:fly={{ y: -20, duration: 500 }}
				out:fly={{ y: 50, duration: 500 }}
			>
				<LiveItem {bItem} />
			</div>
		{/each}
	</div>
</div>

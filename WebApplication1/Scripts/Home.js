$(document).ready(function ()
{
	var history = [];
	var historyIndex = 0;
	var AllTables = $.ajax({ url: "/Home/GetAllTableName", async: false });
	var jsonTable = eval('(' + AllTables.responseText + ')');
	var tables = jsonTable.TableList;
	var tableArray = [];

	for (i = 0; i < tables.length; i++)
	{
		var entity = tables[i];
		tableArray.push(entity.Name)
	}

	var AllEntities = $.ajax({ url: "/Home/GetAllEntityName", async: false });
	var jsonObj = eval('(' + AllEntities.responseText + ')');
	var entities = jsonObj.Entities;
	var entityArray = [];
	var fieldArray = [];

	for (i = 0; i < entities.length; i++)
	{
		var entity = entities[i];
		entityArray.push(entity.Name)
		var fields = entity.Fields;

		for (y = 0 ; y < fields.length; y++)
		{
			fieldArray.push(fields[y].FieldProperties[0].PropertyValue);
		}
	}

	var substringMatcher = function (strs)
	{
		return function findMatches(q, cb)
		{
			var matches, substrRegex;
			matches = [];
			substrRegex = new RegExp(q, 'i');

			$.each(strs, function (i, str)
			{

				if (substrRegex.test(str))
				{
					matches.push({ value: str });
				}

			});
			cb(matches);
		};
	};

	document.getElementById("backToPreviouslySearch").addEventListener("click", function ()
	{
		if (historyIndex > 1)
		{
			historyIndex--;
			$("#SearchTable")[0].value = history[historyIndex - 1];
			GetTableInfo(history[historyIndex - 1], false)
		}

	});

	document.getElementById("backToLaterSearch").addEventListener("click", function ()
	{
		if (historyIndex < history.length)
		{
			historyIndex++;
			$("#SearchTable")[0].value = history[historyIndex - 1];
			GetTableInfo(history[historyIndex - 1], false)
		}
	});

	$('#the-basics .typeahead').typeahead({
		hint: true,
		highlight: true,
		minLength: 1
	},
{
	name: 'Table',
	display: 'value',
	source: substringMatcher(tableArray)
});

	$('#multiple-datasets .typeahead').typeahead({
		highlight: true
	},
	{
		name: 'Entity',
		display: 'value',
		source: substringMatcher(entityArray),
		templates: {
			header: '<h3 class="league-name">Entity</h3>'
		}
	},
	{
		name: 'Fields',
		display: 'value',
		source: substringMatcher(fieldArray),
		templates: {
			header: '<h3 class="league-name">Field</h3>'
		}
	});

	$('#the-basics .typeahead').bind('typeahead:select', function (ev, suggestion)
	{
		GetTableInfo(suggestion.value, true);
	});

	$('#multiple-datasets .typeahead').bind('typeahead:select', function (ev, suggestion)
	{
		GetEntityByName(suggestion.value);
	});

	$("#Search").keyup(function (event)
	{
		if (event.keyCode === 13)
		{
			var entityName = $('#Search')[0].value;
			GetEntityByName(entityName);
		}
	});

	$("#SearchTable").keyup(function (event)
	{
		if (event.keyCode === 13)
		{
			var entityName = $('#SearchTable')[0].value;
			GetTableInfo(entityName, true);
		}
	});

	$("#primarySearchButton").click(function ()
	{
		var entityName = $('#Search')[0].value;
		GetEntityByName(entityName);
	})


	function GetEntityByName(name)
	{
		$.ajax({
			url: "/Home/SearchResult",
			data: { name: name },
			async: false,
			success: function (data)
			{
				$("#TableContent").html(data);
				hljs.configure({ useBR: false });
				$('pre code').each(function (i, block)
				{
					hljs.highlightBlock(block);
				});
				hightLightKeyWords(name);
				HideHistoryNavigationButton();
			}
		});
	};

	function hightLightKeyWords(keyWord)
	{
		$('.hljs-string').each(function ()
		{
			if ($(this)[0].innerText == '"' + keyWord + '"')
			{
				$(this).addClass("keyWord");
			}
		})
	};

	GetTableInfo = function (tableName, writeToHistory)
	{
		$.ajax({
			url: "/Home/TableSearchResult",
			data: { tableName: tableName },
			async: false,
			success: function (data)
			{
				$("#TableContent").html(data);
				hightLightKeyWords(tableName);
				$("#SearchTable")[0].value = tableName;
				if (writeToHistory)
				{
					history.push(tableName);
					historyIndex = history.length;
				}
				ShowHistoryNavigationButton();
			}
		});
	};

	function ShowHistoryNavigationButton()
	{
		$('#HistoryNavigator').each(function ()
		{
			$(this).removeClass("HiddenDiv");
		})
	};

	function HideHistoryNavigationButton()
	{
		$('#HistoryNavigator').each(function ()
		{
			$(this).addClass("HiddenDiv");
		})
	};
});

